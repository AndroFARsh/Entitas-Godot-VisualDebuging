using Godot;

namespace Entitas.Godot;

public partial class Vector2ValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxX;
  private SpinBox _spinBoxY;
  
  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 2;

    _spinBoxX ??= CreateRow("x");
    _spinBoxY ??= CreateRow("y");
    
    _spinBoxX.ValueChanged += OnValueXChanged;
    _spinBoxY.ValueChanged += OnValueYChanged;
  }

  private SpinBox CreateRow(string title)
  {
    SpinBox spinBox = new();
    spinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    spinBox.Prefix = title;
    spinBox.AllowGreater = true;
    spinBox.AllowLesser = true;
    spinBox.Rounded = false;
    spinBox.Step = 0.1;
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
    Vector2 vector = (Vector2)value;
    _spinBoxX.Value = vector.X;
    _spinBoxY.Value = vector.Y;
  }

  private void OnValueXChanged(double x)
  {
    Vector2 vector = ComponentInfo.GetFieldValue<Vector2>(FieldName);
    vector.X = (float)x;
    ComponentInfo.SetFieldValue(FieldName, vector);
  }
  
  private void OnValueYChanged(double y)
  {
    Vector2 vector = ComponentInfo.GetFieldValue<Vector2>(FieldName);
    vector.Y = (float)y;
    ComponentInfo.SetFieldValue(FieldName, vector);
  } 
}