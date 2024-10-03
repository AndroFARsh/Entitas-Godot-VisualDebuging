using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

public partial class EntitasInspectorWindow : Window
{
  private EntitasRoot _entitasRoot;
  private Tree _tree;
  private Control _inspectorContainer;

  private int _prevDebugSystems;
  private readonly List<TreeItem> _removeTreeItem = new ();
  private readonly Dictionary<DebugFeature, TreeItem> _debugSystemsToTreeItem = new ();
  private readonly Dictionary<TreeItem, DebugFeature> _treeItemToDebugSystems = new ();
  
  private readonly Dictionary<TreeItem, IContext> _treeItemToContext = new ();
  private readonly Dictionary<IContext, TreeItem> _contextToTreeItem = new ();
  
  private readonly Dictionary<TreeItem, IEntity> _treeItemToEntity = new ();
  private readonly Dictionary<IEntity, TreeItem> _entityToTreeItem = new ();
  
  private TreeItem _root;
  private TreeItem _contextsRoot;
  private TreeItem _systemsRoot;
  private TreeItem _prevSelected;

  private SystemInspector _systemInspector = new();
  private ContextInspector _contextInspector = new();
  private EntityInspector _entityInspector = new();
  
  private BaseInspector _activeInspector;

  public override void _Ready()
  {
    InitializeTree();
    
    _entitasRoot = EntitasRoot.Global;
    
    _root = _tree.CreateItem();
    _root.SetText(0, "Entities");
    
    _contextsRoot = _tree.CreateItem(_root);
    _contextsRoot.SetText(0, "[Contexts]");
    foreach (IContext context in _entitasRoot.Contexts)
      InitializeContext(_contextsRoot, context);
    
    _systemsRoot = _tree.CreateItem(_root);
    _systemsRoot.SetText(0, "[Systems]");
    
    _entitasRoot.OnEntityCreated += OnEntityChanged;
    _entitasRoot.OnEntityDestroyed += OnEntityDestroyed;

    _tree.ItemSelected += OnItemSelected;
    CloseRequested += OnCloseRequested;
  }

  public override void _Process(double delta)
  {
    UpdateSystemTree();
  }

  private void UpdateSystemTree()
  {
    if (!EntitasRoot.Global.IsFeatureCollectionChanged) return;
    EntitasRoot.Global.IsFeatureCollectionChanged = false;
    
    foreach (DebugFeature systems in _entitasRoot.TopFeatures)
      if (systems.SystemInfo.ParentSystemInfo == null)
        InitializeSystem(_systemsRoot, systems);
    
    _removeTreeItem.Clear();
    foreach ((DebugFeature system, TreeItem treeItem) in _debugSystemsToTreeItem)
      if (!_entitasRoot.AllFeatures.Contains(system))
        _removeTreeItem.Add(treeItem);
   
    foreach (TreeItem treeItem in _removeTreeItem)
    {
      if (_treeItemToDebugSystems.Remove(treeItem, out DebugFeature system))
      {
        _debugSystemsToTreeItem.Remove(system);
        treeItem.GetParent().RemoveChild(treeItem);

        if (treeItem == _prevSelected)
        {
          _tree.SetSelected(_root, 0);
          OnItemSelected();
        }
      }
    }
  }
  
  private void InitializeTree()
  {
    if (GetChildCount() > 0) return;
    
    Title = "Entitas Inspector";
    SetPosition(GetTree().Root.Position);
    SetSize(new Vector2I(1200, 1800));
    SetInitialPosition(WindowInitialPosition.CenterScreenWithMouseFocus);
    
    MarginContainer margin = new();
    margin.AddThemeConstantOverride("margin_top", Consts.Margin);
    margin.AddThemeConstantOverride("margin_left", Consts.Margin);
    margin.AddThemeConstantOverride("margin_bottom", Consts.Margin);
    margin.AddThemeConstantOverride("margin_right", Consts.Margin);
    margin.AnchorsPreset =  (int) Control.LayoutPreset.FullRect;
    margin.AnchorRight = 1;
    margin.AnchorBottom = 1;
    margin.GrowHorizontal = Control.GrowDirection.Both;
    margin.GrowVertical = Control.GrowDirection.Both;
    AddChild(margin);

    HSplitContainer splitContainer = new();
    splitContainer.LayoutMode = 2;
    margin.AddChild(splitContainer);
    
    _tree = new Tree();
    _tree.LayoutMode = 2;
    _tree.CustomMinimumSize = new Vector2(400, 0);
    splitContainer.AddChild(_tree);
    
    PanelContainer panelContainer = new();
    splitContainer.LayoutMode = 2;
    splitContainer.AddChild(panelContainer);
    
    _inspectorContainer = new ScrollContainer();
    splitContainer.LayoutMode = 2;
    splitContainer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
    panelContainer.AddChild(_inspectorContainer);
  }
  
  private void InitializeSystem(TreeItem parent, DebugFeature feature)
  {
    if (!_debugSystemsToTreeItem.TryGetValue(feature, out TreeItem systemItem))
    {
      systemItem = _tree.CreateItem(parent);
      systemItem.SetText(0, feature.Name);

      _debugSystemsToTreeItem.Add(feature, systemItem);
      _treeItemToDebugSystems.Add(systemItem, feature);
    }

    foreach (ISystem childSystem in feature.AllSystems)
      if (childSystem is DebugFeature childDebugSystems)
        InitializeSystem(systemItem, childDebugSystems);
  }

  private void InitializeContext(TreeItem parent, IContext context)
  {
    TreeItem contextItem = _tree.CreateItem(parent);
    contextItem.SetText(0, $"{context.contextInfo.name} Context");
    
    _treeItemToContext.Add(contextItem, context);
    _contextToTreeItem.Add(context, contextItem);
    
    foreach (IEntity entity in _entitasRoot.GetEntities(context))
      InitializeEntity(contextItem, entity);
  }

  private void InitializeEntity(TreeItem contextItem, IEntity entity)
  {
    TreeItem entityItem = _tree.CreateItem(contextItem);
    entityItem.SetText(0, $"Entity_{entity.creationIndex}");
    
    _entityToTreeItem.Add(entity, entityItem);
    _treeItemToEntity.Add(entityItem, entity);
  }
  
  private void OnItemSelected()
  {
    if (_prevSelected == _tree.GetSelected()) return;

    if (_activeInspector != null)
    {
      _activeInspector.CleanUp();
      _inspectorContainer.RemoveChild(_activeInspector);
    }

    _prevSelected = _tree.GetSelected();
    
    if (_treeItemToDebugSystems.TryGetValue(_tree.GetSelected(), out DebugFeature debugSystems))
    {
      _systemInspector.Initialize(debugSystems);
      _inspectorContainer.AddChild(_systemInspector);
      _activeInspector = _systemInspector;
    } 
    else if (_treeItemToContext.TryGetValue(_tree.GetSelected(), out IContext context))
    {
      _contextInspector.Initialize(context);
      _inspectorContainer.AddChild(_contextInspector);
      _activeInspector = _contextInspector;
    }
    else if (_treeItemToEntity.TryGetValue(_tree.GetSelected(), out IEntity entity)) {
      _entityInspector.Initialize(entity);
      _inspectorContainer.AddChild(_entityInspector);
      _activeInspector = _entityInspector;
    }
  }
  
  private void OnEntityChanged(IContext context, IEntity entity)
  {
    if (_contextToTreeItem.TryGetValue(context, out TreeItem contextItem))
      InitializeEntity(contextItem, entity);
  }
  
  private void OnEntityDestroyed(IContext context, IEntity entity)
  {
    if (!_entityToTreeItem.TryGetValue(entity, out TreeItem entityItem)) return;
    
    if (_prevSelected == entityItem)
    {
      _tree.SetSelected(_root, 0);
      OnItemSelected();
    }

    entityItem.GetParent().RemoveChild(entityItem);
          
    _entityToTreeItem.Remove(entity);
    _treeItemToEntity.Remove(entityItem);
          
    entityItem.Free();
  }
  
  private void OnCloseRequested()
  {
    CloseRequested -= OnCloseRequested;
    _tree.ItemSelected -= OnItemSelected;
    
    _entitasRoot.OnEntityCreated -= OnEntityChanged;
    _entitasRoot.OnEntityDestroyed -= OnEntityDestroyed;
    
    _activeInspector?.CleanUp();
    
    _systemInspector.QueueFree();
    _contextInspector.QueueFree();
    _entityInspector.QueueFree();
    
    _debugSystemsToTreeItem.Clear();
    _treeItemToDebugSystems.Clear();
    
    _treeItemToContext.Clear();
    _contextToTreeItem.Clear();

    _treeItemToEntity.Clear();
    _entityToTreeItem.Clear();
    
    QueueFree();
  }
}