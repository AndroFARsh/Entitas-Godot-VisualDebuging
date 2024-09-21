using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

public partial class EntitasInspectorWindow : Window
{
  private enum InspectorType
  {
    System,
    Context,
    Entity,
  }
  
  private Tree _tree;
  private Control _inspectorContainer;
  
  private readonly Dictionary<TreeItem, SystemObserverNode> _treeItemToSystemObserver = new ();
  
  private readonly Dictionary<TreeItem, ContextObserverNode> _treeItemToContextObserver = new ();
  private readonly Dictionary<ContextObserverNode, TreeItem> _contextObserverToTreeItem = new ();
  
  private readonly Dictionary<TreeItem, EntityObserverNode> _treeItemToEntityObserver = new ();
  private readonly Dictionary<EntityObserverNode, TreeItem> _entityObserverToTreeItem = new ();
  
  private TreeItem _root;
  private TreeItem _prevSelected;

  private Dictionary<InspectorType, BaseInspector> _inspectors = new();
  private BaseInspector _inspector;
  
  public override void _Ready() => Initialize();

  private void Initialize()
  {
    Title = "Entitas Inspector";
    SetPosition(GetTree().Root.Position);
    SetMinSize(new Vector2I(800, 600));
    SetInitialPosition(WindowInitialPosition.CenterPrimaryScreen);
    
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
    _tree.CustomMinimumSize = new Vector2(200, 0);
    splitContainer.AddChild(_tree);
    
    PanelContainer panelContainer = new();
    splitContainer.LayoutMode = 2;
    splitContainer.AddChild(panelContainer);
    
    _inspectorContainer = new ScrollContainer();
    splitContainer.LayoutMode = 2;
    splitContainer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
    panelContainer.AddChild(_inspectorContainer);
    
    _root = _tree.CreateItem();
    _root.SetText(0, "Entities");
    
    InitializeTree();

    _inspectors.Add(InspectorType.System, new SystemInspector());
    _inspectors.Add(InspectorType.Context, new ContextInspector());
    _inspectors.Add(InspectorType.Entity, new EntityInspector());
    
    CloseRequested += OnCloseRequested;
    _tree.ItemSelected += OnItemSelected;
  }

  private void InitializeTree()
  {
    foreach (Node node in EntitasRoot.Systems.GetChildren())
      InitializeSystem(_root, node);
    
    foreach (Node node in EntitasRoot.Entities.GetChildren())
      InitializeContext(node);
  }
  
  private void InitializeSystem(TreeItem parent, Node node)
  {
    if (node is not SystemObserverNode systemObserverNode) return;
    
    TreeItem systemItem = _tree.CreateItem(parent);
    systemItem.SetText(0, systemObserverNode.Name);

    _treeItemToSystemObserver.Add(systemItem, systemObserverNode);
    
    foreach (Node childNode in systemObserverNode.GetChildren())
      InitializeSystem(systemItem, childNode);
  }

  private void InitializeContext(Node node)
  {
    if (node is not ContextObserverNode contextObserverNode) return;
    
    TreeItem contextItem = _tree.CreateItem(_root);
    contextItem.SetText(0, $"{contextObserverNode.ContextInfo.name} Context");
    contextObserverNode.OnEntityChanged += OnEntityChanged;

    _treeItemToContextObserver.Add(contextItem, contextObserverNode);
    _contextObserverToTreeItem.Add(contextObserverNode, contextItem);
    
    foreach (Node childNode in contextObserverNode.GetChildren())
      InitializeEntity(contextItem, childNode);
  }

  private void InitializeEntity(TreeItem contextItem, Node node)
  {
    if (node is not EntityObserverNode entityObserverNode) return;

    TreeItem entityItem = _tree.CreateItem(contextItem);
    entityItem.SetText(0, $"Entity_{entityObserverNode.Entity.creationIndex}");
    
    _entityObserverToTreeItem.Add(entityObserverNode, entityItem);
    _treeItemToEntityObserver.Add(entityItem, entityObserverNode);
  }
  
  private void OnItemSelected()
  {
    if (_prevSelected == _tree.GetSelected()) return;

    if (_inspector != null)
    {
      _inspector.CleanUp();
      _inspectorContainer.RemoveChild(_inspector);
    }

    _prevSelected = _tree.GetSelected();
    
    if (_treeItemToSystemObserver.TryGetValue(_tree.GetSelected(), out SystemObserverNode systemObserverNode))
    {
      _inspector = _inspectors[InspectorType.System];
      _inspector.Initialize(systemObserverNode);
      _inspectorContainer.AddChild(_inspector);
    } 
    else if (_treeItemToContextObserver.TryGetValue(_tree.GetSelected(), out ContextObserverNode contextObserverNode))
    {
      _inspector = _inspectors[InspectorType.Context];
      _inspector.Initialize(contextObserverNode);
      _inspectorContainer.AddChild(_inspector);
    }
    else if (_treeItemToEntityObserver.TryGetValue(_tree.GetSelected(), out EntityObserverNode entityObserverNode)) {
      _inspector = _inspectors[InspectorType.Entity];
      _inspector.Initialize(entityObserverNode);
      _inspectorContainer.AddChild(_inspector);
    }
  }
  
  private void OnEntityChanged(EntityActionType action, ContextObserverNode contextObserver, EntityObserverNode entity)
  {
    switch (action)
    {
      case EntityActionType.Created:
        if (_contextObserverToTreeItem.TryGetValue(contextObserver, out TreeItem contextItem))
        {
          InitializeEntity(contextItem, entity);
        }

        break;
      case EntityActionType.Destroyed:
        if (_entityObserverToTreeItem.TryGetValue(entity, out TreeItem entityItem))
        {
          if (_prevSelected == entityItem)
          {
            _tree.SetSelected(_root, 0);
            OnItemSelected();
          }

          entityItem.GetParent().RemoveChild(entityItem);
          
          _entityObserverToTreeItem.Remove(entity);
          _treeItemToEntityObserver.Remove(entityItem);
          
          entityItem.Free();
        }

        break;
    }
  }
  
  private void OnCloseRequested()
  {
    foreach (ContextObserverNode contextObserverNode in _contextObserverToTreeItem.Keys)
    {
      contextObserverNode.OnEntityChanged -= OnEntityChanged;
    }
    _tree.ItemSelected -= OnItemSelected;
    _tree.Clear();
    
    _inspector?.CleanUp();
    
    _treeItemToSystemObserver.Clear();
    
    _treeItemToContextObserver.Clear();
    _contextObserverToTreeItem.Clear();

    _treeItemToEntityObserver.Clear();
    _contextObserverToTreeItem.Clear();
    
    QueueFree();
  }
}