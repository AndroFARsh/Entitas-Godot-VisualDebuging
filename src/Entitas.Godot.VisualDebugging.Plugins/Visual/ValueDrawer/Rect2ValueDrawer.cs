using Godot;

namespace Entitas.Godot;

public partial class Rect2ValueDrawer : BaseValueDrawer
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
    spinBox.Rounded = false;
    spinBox.Step = 0.0001;
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
    Rect2 rect2 = (Rect2)value;
    _spinBoxX.Value = rect2.Position.X;
    _spinBoxY.Value = rect2.Position.Y;
    _spinBoxW.Value = rect2.Size.X;
    _spinBoxH.Value = rect2.Size.Y;
  }

  private void OnValueXChanged(double x)
  {
    Rect2 rect = ComponentInfo.GetFieldValue<Rect2>(FieldName);
    Vector2 position = rect.Position;
    position.X = (float)x;
    rect.Position = position;
    ComponentInfo.SetFieldValue(FieldName, rect);
  }
  
  private void OnValueYChanged(double y)
  {
    Rect2 rect = ComponentInfo.GetFieldValue<Rect2>(FieldName);
    Vector2 position = rect.Position;
    position.Y = (float)y;
    rect.Position = position;
    ComponentInfo.SetFieldValue(FieldName, rect);
  } 
  
  private void OnValueWChanged(double w)
  {
    Rect2 rect = ComponentInfo.GetFieldValue<Rect2>(FieldName);
    Vector2 size = rect.Size;
    size.X = (float)w;
    rect.Size = size;
    ComponentInfo.SetFieldValue(FieldName, rect);
  }
  
  private void OnValueHChanged(double h)
  {
    Rect2 rect = ComponentInfo.GetFieldValue<Rect2>(FieldName);
    Vector2 size = rect.Size;
    size.Y = (float)h;
    rect.Size = size;
    ComponentInfo.SetFieldValue(FieldName, rect);
  }
}