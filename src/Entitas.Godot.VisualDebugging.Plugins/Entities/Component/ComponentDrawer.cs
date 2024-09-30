using System;
using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

public partial class ComponentDrawer : MarginContainer
{ 
  private ComponentInfo _componentInfo;
  private IEntity _entity;
  private Dictionary<string, BaseValueDrawer> _valueDrawers = new();

  private ColorRect _bg;
  private BaseButton _collapseButton;
  private Label _nameLabel;
  private Button _removeComponentButton;
  private Container _valuesContainer;
  
  public ComponentInfo ComponentInfo => _componentInfo;
  public bool ShouldCollapse => _collapseButton.ButtonPressed && !_componentInfo.IsFlaggableComponent;
  
  public void Initialize(IEntity entity, ComponentInfo componentInfo)
  {
    InitializeTree();
    
    _bg.Color = Color.FromHsv(Random.Shared.NextSingle(), 1, 0.4f);
    _entity = entity;
    _componentInfo = componentInfo;
    _nameLabel.Name = componentInfo.Name;
    _nameLabel.Text = componentInfo.Name;
    _valuesContainer.Visible = ShouldCollapse;
    
    _collapseButton.Pressed += OnCollapsePressed;
    _removeComponentButton.Pressed += OnRemoveComponentPressed;
    
    _entity.OnComponentReplaced += OnComponentReplaced;
    
    foreach (string fieldName in componentInfo.FieldNames)
      CreateDrawer(componentInfo, fieldName);
  }
  
  private void InitializeTree()
  {
    if (GetChildCount() > 0) return;
    
    AnchorsPreset = (int) LayoutPreset.FullRect;
    AnchorRight = 1;
    AnchorBottom = 1;
    GrowHorizontal = GrowDirection.Both;
    GrowVertical = GrowDirection.Both;
    SizeFlagsHorizontal = SizeFlags.ExpandFill;

    _bg = new ColorRect();
    _bg.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    _bg.SizeFlagsVertical = SizeFlags.ExpandFill;
    _bg.Color = new Color(1, 1, 1, 0.1f);
    AddChild(_bg);

    VBoxContainer container = new();
    container.AddThemeConstantOverride("separation", Consts.Margin);
    container.LayoutMode = 2;
    AddChild(container);

    MarginContainer titleMarginContainer = new();
    titleMarginContainer.AddThemeConstantOverride("margin_top", Consts.Margin);
    titleMarginContainer.AddThemeConstantOverride("margin_left", Consts.Margin);
    titleMarginContainer.AddThemeConstantOverride("margin_bottom", Consts.Margin);
    titleMarginContainer.AddThemeConstantOverride("margin_right", Consts.Margin);
    container.AddChild(titleMarginContainer);

    HBoxContainer titleContainer = new();
    titleContainer.AddThemeConstantOverride("separation", Consts.Margin);
    titleMarginContainer.AddChild(titleContainer);

    _collapseButton = new CheckButton();
    _collapseButton.ButtonPressed = true;
    titleContainer.AddChild(_collapseButton);

    _nameLabel = new Label();
    _nameLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    _nameLabel.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    titleContainer.AddChild(_nameLabel);

    _removeComponentButton = new Button();
    _removeComponentButton.CustomMinimumSize = new Vector2(45, 45);
    _removeComponentButton.Text = "—";
    titleContainer.AddChild(_removeComponentButton);

    _valuesContainer = new VBoxContainer();
    _valuesContainer.AddThemeConstantOverride("separation", Consts.Margin);
    container.AddChild(_valuesContainer);
  }

  public void CleanUp()
  {
    foreach ((string fieldName, BaseValueDrawer drawer) in _valueDrawers)
    {
      _valuesContainer.RemoveChild(drawer);
      drawer.CleanUp();
      
      Type type = _componentInfo.GetFieldType(fieldName);
      EntitasRoot.Global.Pool.Retain(type, drawer);
    }
    _valueDrawers.Clear();
    
    _collapseButton.Pressed -= OnCollapsePressed;
    _removeComponentButton.Pressed -= OnRemoveComponentPressed;
    _entity.OnComponentReplaced -= OnComponentReplaced;
  }
  
  private void CreateDrawer(ComponentInfo componentInfo, string fieldName)
  { 
    Type type = _componentInfo.GetFieldType(fieldName);
    if (!EntitasRoot.Global.Pool.Request(type, out BaseValueDrawer drawer))
      drawer = type.CreateDrawer();
    
    drawer.Initialize(componentInfo, fieldName);
    
    _valueDrawers.Add(fieldName, drawer);
    _valuesContainer.AddChild(drawer);
  }

  private void OnCollapsePressed() => _valuesContainer.Visible = ShouldCollapse;
  
  private void OnComponentReplaced(IEntity entity, int index, IComponent previousComponent, IComponent newComponent)
  {
    ComponentInfo componentInfo = new(entity, index, newComponent);
    if (_componentInfo.Name != componentInfo.Name) return;
    
    foreach (string fieldName in componentInfo.FieldNames)
      if (_valueDrawers.TryGetValue(fieldName, out BaseValueDrawer drawer))
        drawer.UpdateValue(componentInfo.GetFieldValue(fieldName));
  }

  private void OnRemoveComponentPressed()
  {
    _componentInfo.Entity.RemoveComponent(_componentInfo.Index);
    _componentInfo = null;
  }
}