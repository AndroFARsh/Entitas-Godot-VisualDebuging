using Godot;

namespace Entitas.Godot;

public partial class Vector3IValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxX;
  private SpinBox _spinBoxY;
  private SpinBox _spinBoxZ;
  
  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 3;

    _spinBoxX ??= CreateRow("x");
    _spinBoxY ??= CreateRow("y");
    _spinBoxZ ??= CreateRow("z");

    _spinBoxX.ValueChanged += OnValueXChanged;
    _spinBoxY.ValueChanged += OnValueYChanged;
    _spinBoxZ.ValueChanged += OnValueZChanged;
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
    _spinBoxZ.ValueChanged -= OnValueZChanged;
  }

  public override void UpdateValue(object value)
  {
    Vector3I vector = (Vector3I)value;
    _spinBoxX.Value = vector.X;
    _spinBoxY.Value = vector.Y;
    _spinBoxZ.Value = vector.Z;
  }

  private void OnValueXChanged(double x)
  {
    Vector3I vector = ComponentInfo.GetFieldValue<Vector3I>(FieldName);
    vector.X = (int)x;
    ComponentInfo.SetFieldValue(FieldName, vector);
  }
  
  private void OnValueYChanged(double y)
  {
    Vector3I vector = ComponentInfo.GetFieldValue<Vector3I>(FieldName);
    vector.Y = (int)y;
    ComponentInfo.SetFieldValue(FieldName, vector);
  }
  
  private void OnValueZChanged(double z)
  {
    Vector3I vector = ComponentInfo.GetFieldValue<Vector3I>(FieldName);
    vector.Z = (int)z;
    ComponentInfo.SetFieldValue(FieldName, vector);
  } 
}