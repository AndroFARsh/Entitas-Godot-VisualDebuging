using Godot;

namespace Entitas.Godot;

public partial class Rect2IValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxX;
  private SpinBox _spinBoxY;
  private SpinBox _spinBoxW;
  private SpinBox _spinBoxH;
  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 2;

    _spinBoxX ??= CreateRow("x");
    _spinBoxY ??= CreateRow("y");
    _spinBoxW ??= CreateRow("w");
    _spinBoxH ??= CreateRow("h");
    
    _spinBoxX.ValueChanged += OnValueXChanged;
    _spinBoxY.ValueChanged += OnValueYChanged;
    _spinBoxW.ValueChanged += OnValueWChanged;
    _spinBoxH.ValueChanged += OnValueHChanged;
  }

  private SpinBox CreateRow(string title)
  {
    SpinBox spinBox = new();
    spinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    spinBox.Prefix = title;
    spinBox.AllowGreater = true;
    spinBox.AllowLesser = true;
    spinBox.Rounded = true;
    spinBox.Step = 1;
    GridContainer.AddChild(spinBox);
    
    return spinBox;
  }

  protected override void CleanUpDrawer()
  {
    _spinBoxX.ValueChanged -= OnValueXChanged;
    _spinBoxY.ValueChanged -= OnValueYChanged;
    _spinBoxW.ValueChanged -= OnValueWChanged;
    _spinBoxH.ValueChanged -= OnValueHChanged;
  }

  public override void UpdateValue(object value)
  {
    Rect2I rect2 = (Rect2I)value;
    _spinBoxX.Value = rect2.Position.X;
    _spinBoxY.Value = rect2.Position.Y;
    _spinBoxW.Value = rect2.Size.X;
    _spinBoxH.Value = rect2.Size.Y;
  }

  private void OnValueXChanged(double x)
  {
    Rect2I rect = ComponentInfo.GetFieldValue<Rect2I>(FieldName);
    Vector2I position = rect.Position;
    position.X = (int)x;
    rect.Position = position;
    ComponentInfo.SetFieldValue(FieldName, rect);
  }
  
  private void OnValueYChanged(double y)
  {
    Rect2I rect = ComponentInfo.GetFieldValue<Rect2I>(FieldName);
    Vector2I position = rect.Position;
    position.Y = (int)y;
    rect.Position = position;
    ComponentInfo.SetFieldValue(FieldName, rect);
  } 
  
  private void OnValueWChanged(double w)
  {
    Rect2I rect = ComponentInfo.GetFieldValue<Rect2I>(FieldName);
    Vector2I size = rect.Size;
    size.X = (int)w;
    rect.Size = size;
    ComponentInfo.SetFieldValue(FieldName, rect);
  }
  
  private void OnValueHChanged(double h)
  {
    Rect2I rect = ComponentInfo.GetFieldValue<Rect2I>(FieldName);
    Vector2I size = rect.Size;
    size.Y = (int)h;
    rect.Size = size;
    ComponentInfo.SetFieldValue(FieldName, rect);
  }
}