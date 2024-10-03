using System;
using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

[GlobalClass]
public partial class EntitasRoot : CanvasLayer
{
  private const int MenuInspectorId = 1234000;
  private const int MenuDebugSystemsId = MenuInspectorId + 1;

  private static EntitasRoot _global;

  public static EntitasRoot Global => _global ??= CreateNewInstance();

  private readonly Dictionary<IContext, HashSet<IEntity>> _contextToEntity = new();
  private readonly Dictionary<IContext, HashSet<IGroup>> _contextToGroup = new();
  private readonly Dictionary<IEntity, IContext> _entityToContext = new();
  
  private readonly Dictionary<long, SystemInfo> _allDebugFeatures = new();
  private readonly HashSet<IContext> _contexts = new();
  private readonly HashSet<DebugFeature> _allFeatures = new();
  
  private readonly HashSet<DebugFeature> _topFeatures = new();
  private readonly NodePool _pool = new();
  
  private Window _window;
  private MenuButton _debugMenuButton;
  private PopupMenu _debugSystemPopup;

  public bool IsFeatureCollectionChanged { get; set; }
  
  public HashSet<IContext> Contexts => _contexts;
  public HashSet<DebugFeature> TopFeatures => _topFeatures;
  public HashSet<DebugFeature> AllFeatures => _allFeatures;
  public NodePool Pool => _pool;

  public event Action<IContext, IEntity> OnEntityCreated;
  public event Action<IContext, IEntity> OnEntityDestroyed;
  public event Action<IContext, IGroup> OnGroupCreated;
  
  public static void Register(IContext context)
  {
    EntitasRoot global = Global;

    context.OnEntityCreated += global.EntityCreated;
    context.OnEntityDestroyed += global.EntityDestroyed;
    context.OnGroupCreated += global.GroupCreated;

    global._contexts.Add(context);
  }

  public static void Pause()
  {
    foreach (DebugFeature feature in Global._topFeatures)
      feature.SystemInfo.IsActive = false;
  }

  public static void Resume()
  {
    foreach (DebugFeature feature in Global._topFeatures)
      feature.SystemInfo.IsActive = true;
  }

  public static void RegisterFeature(DebugFeature feature)
  {
    Global._allFeatures.Add(feature);
    Global._topFeatures.Add(feature);
    RegisterSystem(feature.SystemInfo);
    Global.IsFeatureCollectionChanged = true;
  }
  
  public static void RegisterSystem(SystemInfo systemInfo)
  {
    if (systemInfo.IsDebugSystems)
      Global.RefreshDebugSystemMenu(systemInfo, true);  
  }

  public static void UnregisterTopFeature(DebugFeature feature)
  {
    Global._topFeatures.Remove(feature);
    Global.IsFeatureCollectionChanged = true;
  }

  public static void UnregisterSystem(SystemInfo systemInfo)
  {
    if (systemInfo.IsDebugSystems)
      Global.RefreshDebugSystemMenu(systemInfo, false);
  }

  public static void UnregisterFeature(DebugFeature feature)
  {
    Global._allFeatures.Remove(feature);
    Global._topFeatures.Remove(feature);
    UnregisterSystem(feature.SystemInfo);
    Global.IsFeatureCollectionChanged = true;
  }

  public IContext GetContext(IEntity entity) => _entityToContext.GetValueOrDefault(entity);

  public HashSet<IEntity> GetEntities(IContext context)
  {
    if (!_contextToEntity.TryGetValue(context, out HashSet<IEntity> _))
      _contextToEntity.Add(context, new HashSet<IEntity>());

    return _contextToEntity[context];
  }
  
  public HashSet<IGroup> GetGroups(IContext context)
  {
    if (!_contextToGroup.TryGetValue(context, out HashSet<IGroup> _))
      _contextToGroup.Add(context, new HashSet<IGroup>());

    return _contextToGroup[context];
  }
  
  private void GroupCreated(IContext context, IGroup group)
  {
    if (!_contextToGroup.TryGetValue(context, out HashSet<IGroup> groups))
      groups = new HashSet<IGroup>();
    
    groups.Add(group);
    
    OnGroupCreated?.Invoke(context, group);
  }

  private void EntityDestroyed(IContext context, IEntity entity)
  {
    if (_contextToEntity.TryGetValue(context, out HashSet<IEntity> entities))
      entities.Remove(entity);
    
    _entityToContext.Remove(entity);
    GetEntities(context).Remove(entity);
    
    OnEntityDestroyed?.Invoke(context, entity);
  }

  private void EntityCreated(IContext context, IEntity entity)
  {
    if (!_contextToEntity.TryGetValue(context, out HashSet<IEntity> entities))
      entities = new HashSet<IEntity>();
    
    entities.Add(entity);
    _entityToContext.Add(entity, context);
    GetEntities(context).Add(entity);
    
    OnEntityCreated?.Invoke(context, entity);
  }

  private static EntitasRoot CreateNewInstance()
  {
    EntitasRoot newInstance = new();
    Node root = ((SceneTree)Engine.GetMainLoop()).CurrentScene;
    root.AddChild(newInstance);
    return newInstance;
  }
  
  

  public override void _EnterTree()
  {
    if (_global == null)
    {
      Initialize();
      _global = this;
      return;
    }
    QueueFree();
  }
  
  private void OnInspectorWindowClosed()
  {
    _window.CloseRequested -= OnInspectorWindowClosed;
    _window.GetParent().RemoveChild(_window);
    _window.QueueFree();
    _window = null;
  }
  
  public override void _Notification(int what)
  {
    if (what == NotificationPredelete)
    {
      if (_global == this)
      {
        foreach (DebugFeature system in _allFeatures)
          system.Dispose();
        
        _pool.Dispose();
        _allFeatures.Clear();
        _topFeatures.Clear();
        _window?.QueueFree();
        _debugMenuButton?.QueueFree();
        _window = null;
        _debugMenuButton = null;
        _global = null;
        GetParent()?.RemoveChild(this);
        QueueFree();
      }
    }
  }
  
  private void Initialize()
  {
    MenuBar menuBar = new();
    menuBar.PreferGlobalMenu = true;
    
    AddChild(menuBar);
    _debugMenuButton = new MenuButton();
    _debugMenuButton.Text = "Debug Tools";
    _debugMenuButton.GetPopup().PreferNativeMenu = false;
    _debugMenuButton.GetPopup().SubmenuPopupDelay = 3;
    _debugMenuButton.GetPopup().AddItem("Entitas Inspector", MenuInspectorId);
    
    _debugSystemPopup = new PopupMenu();
    _debugSystemPopup.AllowSearch = true;
    _debugSystemPopup.PreferNativeMenu = false;
    _debugMenuButton.GetPopup().AddSubmenuNodeItem("Debug Systems", _debugSystemPopup, MenuDebugSystemsId);
    
    _debugSystemPopup.IdPressed += OnDebugSystemItemPressed;

    _debugMenuButton.GetPopup().IdPressed += OnMenuItemPressed;
    
    menuBar.AddChild(_debugMenuButton);
    Name = "[Entitas]";
  }

  private void OnDebugSystemItemPressed(long id)
  {
    if (_allDebugFeatures.TryGetValue(id, out SystemInfo systemInfo))
    {
      int index = _debugSystemPopup.GetItemIndex((int)id);
      systemInfo.IsActive = !systemInfo.IsActive;
      _debugSystemPopup.SetItemChecked(index, systemInfo.IsActive);
    }
  }

  private void RefreshDebugSystemMenu(SystemInfo systemInfo, bool created)
  {
    int id = systemInfo.GetHashCode();
    if (created)
    {
      _allDebugFeatures.Add(id, systemInfo);
      _debugSystemPopup.AddCheckItem(systemInfo.SystemName, id);
      _debugSystemPopup.SetItemChecked(_debugSystemPopup.GetItemIndex(id), systemInfo.IsActive); 
    }
    else
    {
      _allDebugFeatures.Remove(id);
      _debugSystemPopup.RemoveItem(_debugSystemPopup.GetItemIndex(id));
    }
  }

  private void OnMenuItemPressed(long id)
  {
    if (id == MenuInspectorId && _window == null)
    {
      GetViewport().SetEmbeddingSubwindows(false);
      _window = new EntitasInspectorWindow();
      _window.CloseRequested += OnInspectorWindowClosed;
      AddChild(_window);
    }
  }
}