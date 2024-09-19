using System;
using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

public partial class EntityInspector : BaseInspector
{
  [Export] private BaseButton _destroyEntityButton;
  [Export] private BaseButton _cloneEntityButton;
  [Export] private OptionButton _componentsButton;
  [Export] private TextEdit _searchTextEdit;
  [Export] private BaseButton _clearSearchButton;
  [Export] private Container _componentsContainer;
  
  private ContextInfo _contextInfo;
  private EntityObserverNode _entityObserverNode;
  private PackedScene _componentPackedScene = ResourceLoader.Load<PackedScene>(VdConst.ComponentResourcePath);
  
  private readonly Dictionary<ComponentInfo, ComponentDrawer> _drawers = new();
  
  public override void Initialize(Node node)
  {
    if (node == _entityObserverNode) return;
    _entityObserverNode = (EntityObserverNode)node;
    _contextInfo = _entityObserverNode.Context.contextInfo;
    
    _componentsButton.ItemSelected += OnComponentAddAction;
    _entityObserverNode.ComponentInfoAction += OnComponentInfoAction;
    _destroyEntityButton.Pressed += OnDestroyEntityPressed;
    _cloneEntityButton.Pressed += OnCloneEntityPressed;
    _searchTextEdit.TextChanged += OnSearchTextChanged;
    _clearSearchButton.Pressed += OnClearSearchPressed;

    FillComponentDropDown();

    foreach (ComponentInfo componentInfo in _entityObserverNode.Components)
    {
      ComponentDrawer componentDrawer = CreateComponentDrawer(_entityObserverNode, componentInfo);
      _componentsContainer.AddChild(componentDrawer);
      _drawers.Add(componentInfo, componentDrawer);
    }

    Visible = true;
  }

  private void FillComponentDropDown()
  {
    _componentsButton.Clear();
    
    IEntity entity = _entityObserverNode?.Entity;
    if (entity == null) return;

    int index = 0;
    foreach (Type componentType in _contextInfo.componentTypes)
    {
      if (!entity.HasComponent(componentType))
      {
        _componentsButton.AddItem(componentType.FullName, index);
      }

      index++;
    }
    _componentsButton.Selected = -1;
  }

  private ComponentDrawer CreateComponentDrawer(EntityObserverNode entityObserverNode, ComponentInfo componentInfo)
  {
    if (!EntitasRoot.Pool.Request(typeof(ComponentInfo), out ComponentDrawer drawer)) 
      drawer = _componentPackedScene.Instantiate<ComponentDrawer>();  
    
    drawer.Initialize(entityObserverNode, componentInfo);    
    return drawer;
  }

  public override void CleanUp()
  {
    if (_entityObserverNode == null) return;

    _componentsButton.Clear();

    foreach ((ComponentInfo _, ComponentDrawer drawer)  in _drawers)
    {
      drawer.CleanUp();
      _componentsContainer.RemoveChild(drawer);
      
      EntitasRoot.Pool.Retain(typeof(ComponentInfo), drawer);
    }
    _drawers.Clear();

    _componentsButton.ItemSelected -= OnComponentAddAction;
    _entityObserverNode.ComponentInfoAction -= OnComponentInfoAction;
    _destroyEntityButton.Pressed -= OnDestroyEntityPressed;
    _cloneEntityButton.Pressed -= OnCloneEntityPressed;
    _searchTextEdit.TextChanged -= OnSearchTextChanged;
    _clearSearchButton.Pressed -= OnClearSearchPressed;
    
    _entityObserverNode = null;
    _contextInfo = null;
    Visible = false;
  }

  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    _componentPackedScene = null;
  }

  private void OnClearSearchPressed()
  {
    _searchTextEdit.Text = "";
    OnSearchTextChanged();
  }

  private void OnComponentInfoAction(ComponentActionType action, ComponentInfo componentInfo)
  {
    switch (action)
    {
      case ComponentActionType.Added:
      {
        ComponentDrawer componentDrawer = CreateComponentDrawer(_entityObserverNode, componentInfo);
        _componentsContainer.AddChild(componentDrawer);
        _drawers.Add(componentInfo, componentDrawer);
        
        FillComponentDropDown();
        break;
      }
      case ComponentActionType.Removed:
      {
        if (_drawers.TryGetValue(componentInfo, out ComponentDrawer drawer))
        {
          drawer.CleanUp();
          EntitasRoot.Pool.Retain(typeof(ComponentInfo), drawer);
          _drawers.Remove(componentInfo);
          _componentsContainer.RemoveChild(drawer);
          
          FillComponentDropDown();
        }
        break;
      }
    }
  }

  private void OnDestroyEntityPressed() => _entityObserverNode.Entity.Destroy();

  private void OnSearchTextChanged()
  {
    foreach (Node node in _componentsContainer.GetChildren())
      if (node is ComponentDrawer componentNode)
      {
        ComponentInfo componentInfo = componentNode.ComponentInfo;
        string componentName = componentInfo?.Name ?? "";
        componentNode.Visible = componentName.Contains(_searchTextEdit.Text, StringComparison.CurrentCultureIgnoreCase);
      }
  }
  
  private void OnCloneEntityPressed()
  {
    IEntity newEntity = _entityObserverNode.Context.CreateEntity();
    foreach (ComponentInfo componentInfo in _entityObserverNode.Components)
    {
      ComponentInfo newComponentInfo = new(newEntity, componentInfo.Index, Activator.CreateInstance(componentInfo.Type) as IComponent);
      foreach (string fieldName in componentInfo.FieldNames)
        newComponentInfo.SetFieldValue(fieldName, componentInfo.GetFieldValue(fieldName));
      
      newEntity.AddComponent(newComponentInfo.Index, newComponentInfo.Component);
    }
  }
  
  private void OnComponentAddAction(long i)
  {
    int index = _componentsButton.GetItemId((int)i);
    IEntity entity = _entityObserverNode.Entity;
    
    if (!entity.HasComponent(index))
    {
      Type componentType = _contextInfo.componentTypes[index];
      ComponentInfo componentInfo = new(entity, index, Activator.CreateInstance(componentType) as IComponent);
      
      entity.AddComponent(componentInfo.Index, componentInfo.Component);
    }

    _componentsButton.Selected = -1;
  }
}