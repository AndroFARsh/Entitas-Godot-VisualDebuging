using System;
using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

[GlobalClass]
public partial class EntitasRoot : CanvasLayer
{
  private const int MenuInspectorId = 122412;

  private static EntitasRoot _global;

  public static EntitasRoot Global => _global ??= CreateNewInstance();

  private readonly Dictionary<IContext, HashSet<IEntity>> _contextToEntity = new();
  private readonly Dictionary<IContext, HashSet<IGroup>> _contextToGroup = new();
  private readonly Dictionary<IEntity, IContext> _entityToContext = new();
  
  private readonly HashSet<IContext> _contexts = new();
  private readonly HashSet<DebugSystems> _allSystems = new();
  private readonly HashSet<DebugSystems> _topSystems = new();
  private readonly NodePool _pool = new();
  
  private Window _window;
  private MenuButton _debugMenuButton;

  public bool IsSystemChanged { get; set; }
  
  public HashSet<IContext> Contexts => _contexts;
  public HashSet<DebugSystems> TopSystems => _topSystems;
  public HashSet<DebugSystems> AllSystems => _allSystems;
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
  
  public static void RegisterSystem(DebugSystems systems)
  {
    Global._allSystems.Add(systems);
    Global._topSystems.Add(systems);
    Global.IsSystemChanged = true;
  }

  public static void UnregisterTopSystem(DebugSystems systems)
  {
    Global._topSystems.Remove(systems);
    Global.IsSystemChanged = true;
  }
  
  public static void UnregisterSystem(DebugSystems systems)
  {
    Global._allSystems.Remove(systems);
    Global._topSystems.Remove(systems);
    Global.IsSystemChanged = true;
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
        foreach (DebugSystems system in _allSystems)
          system.Dispose();
        
        _pool.Dispose();
        _allSystems.Clear();
        _topSystems.Clear();
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
    _debugMenuButton.GetPopup().AddItem("Entitas Inspector", MenuInspectorId);
    _debugMenuButton.GetPopup().IdPressed += OnMenuItemPressed;
    menuBar.AddChild(_debugMenuButton);
    Name = "[Entitas]";
  }

  private void OnMenuItemPressed(long id)
  {
    _debugMenuButton.GetPopup().PreferNativeMenu = true;
    if (id == MenuInspectorId && _window == null)
    {
      GetViewport().SetEmbeddingSubwindows(false);
      _window = new EntitasInspectorWindow();
      _window.CloseRequested += OnInspectorWindowClosed;
      AddChild(_window);
    }
  }
}