using System;
using Godot;

namespace Entitas.Godot;

public partial class ValueRootDrawer : Control
{ 
  [Export] private Label _title;
  [Export] private ColorRect _background;
  [Export] private Control _contentContainer;
  
  private BaseValueDrawer _drawer;
  
  public void Initialize(ComponentInfo componentInfo, string fieldName, Color bg)
  {
    _title.Text = fieldName;
    _background.Color = bg;
    
    Type type = componentInfo.GetFieldType(fieldName);
    if (!EntitasRoot.Pool.Request(type, out _drawer))
      _drawer = type.CreateDrawer();
    
    _drawer.Initialize(componentInfo, fieldName);
    _contentContainer.AddChild(_drawer);
  }

  public void CleanUp()
  {
    if (_drawer == null) return;
    
    Type type = _drawer.ComponentInfo.GetFieldType(_drawer.FieldName);
    EntitasRoot.Pool.Retain(type, _drawer);
    
    _contentContainer.RemoveChild(_drawer);
    _drawer.CleanUp();

    _drawer = null;
  }

  public void UpdateValue(object value) => _drawer?.UpdateValue(value);
}