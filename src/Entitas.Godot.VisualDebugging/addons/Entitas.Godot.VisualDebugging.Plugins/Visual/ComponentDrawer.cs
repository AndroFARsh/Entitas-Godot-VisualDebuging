using System;
using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

public partial class ComponentDrawer : Control
{ 
  private ComponentInfo _componentInfo;
  private EntityObserverNode _entityObserverNode;
  private Dictionary<string, ValueRootDrawer> _valueDrawers = new();
  private PackedScene _valueRootPackedScene = ResourceLoader.Load<PackedScene>(VdConst.ValueResourcePath);

  [Export] private ColorRect _background;
  [Export] private BaseButton _collapseButton;
  [Export] private Label _nameLabel;
  [Export] private BaseButton _removeComponentButton;
  [Export] private Container _valuesContainer;
  
  public ComponentInfo ComponentInfo => _componentInfo;
  public bool ShouldCollapse => _collapseButton.ButtonPressed && !_componentInfo.IsFlaggableComponent;
  
  public void Initialize(EntityObserverNode entityObserverNode, ComponentInfo componentInfo)
  {
    _background.Color = Color.FromHsv(Random.Shared.NextSingle(), 1, 0.2f);
    _entityObserverNode = entityObserverNode;
    _componentInfo = componentInfo;
    _nameLabel.Name = componentInfo.Name;
    _nameLabel.Text = componentInfo.Name;
    _valuesContainer.Visible = ShouldCollapse;
    
    _collapseButton.Pressed += OnCollapsePressed;
    _removeComponentButton.Pressed += OnRemoveComponentPressed;
    _entityObserverNode.ComponentInfoAction += OnComponentInfoAction;

    int i = 0;
    foreach (string fieldName in componentInfo.FieldNames)
      CreateDrawer(componentInfo, fieldName, i++ % 2 == 0);
  }

  public void CleanUp()
  {
    foreach (ValueRootDrawer drawer in _valueDrawers.Values)
    {
      _valuesContainer.RemoveChild(drawer);
      drawer.CleanUp();
      EntitasRoot.Pool.Retain(typeof(ValueRootDrawer), drawer);
    }
    _valueDrawers.Clear();
    
    _collapseButton.Pressed -= OnCollapsePressed;
    _removeComponentButton.Pressed -= OnRemoveComponentPressed;
    _entityObserverNode.ComponentInfoAction -= OnComponentInfoAction;
  }
  
  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    _valueRootPackedScene = null;
  }
  
  private void CreateDrawer(ComponentInfo componentInfo, string fieldName, bool even)
  { 
    if (!EntitasRoot.Pool.Request(typeof(ValueRootDrawer), out ValueRootDrawer drawer))
      drawer = _valueRootPackedScene.Instantiate<ValueRootDrawer>();
    
    drawer.Initialize(componentInfo, fieldName, new Color(1, 1, 1, even ? 0 : 0.1f));
    
    _valueDrawers.Add(fieldName, drawer);
    _valuesContainer.AddChild(drawer);
  }

  private void OnCollapsePressed() => _valuesContainer.Visible = ShouldCollapse;
  
  private void OnComponentInfoAction(ComponentActionType action, ComponentInfo componentInfo)
  {
    if (action != ComponentActionType.Replaced || _componentInfo.Name != componentInfo.Name) return;
    
    foreach (string fieldName in componentInfo.FieldNames)
      if (_valueDrawers.TryGetValue(fieldName, out ValueRootDrawer drawer))
        drawer.UpdateValue(componentInfo.GetFieldValue(fieldName));
  }

  private void OnRemoveComponentPressed()
  {
    _componentInfo.Entity.RemoveComponent(_componentInfo.Index);
    _componentInfo = null;
  }
}