using Godot;

namespace Entitas.Godot;

public partial class Vector2IValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxX;
  private SpinBox _spinBoxY;
  
  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 2;

    _spinBoxX ??= CreateRow("x");
    _spinBoxY ??= CreateRow("y");
    
    _spinBoxY.ValueChanged += OnValueYChanged;
    _spinBoxX.ValueChanged += OnValueXChanged;
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
  }

  public override void UpdateValue(object value)
  {
    Vector2I vector = (Vector2I)value;
    _spinBoxX.Value = vector.X;
    _spinBoxY.Value = vector.Y;
  }

  private void OnValueXChanged(double x)
  {
    Vector2I vector = ComponentInfo.GetFieldValue<Vector2I>(FieldName);
    vector.X = (int)x;
    ComponentInfo.SetFieldValue(FieldName, vector);
  }
  
  private void OnValueYChanged(double y)
  {
    Vector2I vector = ComponentInfo.GetFieldValue<Vector2I>(FieldName);
    vector.Y = (int)y;
    ComponentInfo.SetFieldValue(FieldName, vector);
  } 
}