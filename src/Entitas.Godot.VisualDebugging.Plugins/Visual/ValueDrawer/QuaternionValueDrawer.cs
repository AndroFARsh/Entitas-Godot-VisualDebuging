using Godot;

namespace Entitas.Godot;

public partial class QuaternionValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxX;
  private SpinBox _spinBoxY;
  private SpinBox _spinBoxZ;
  private SpinBox _spinBoxW;
  
  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 4;

    _spinBoxX ??= CreateRow("x");
    _spinBoxY ??= CreateRow("y");
    _spinBoxZ ??= CreateRow("z");
    _spinBoxW ??= CreateRow("w");
    
    _spinBoxX.ValueChanged += OnValueXChanged;
    _spinBoxY.ValueChanged += OnValueYChanged;
    _spinBoxZ.ValueChanged += OnValueZChanged;
    _spinBoxW.ValueChanged += OnValueWChanged;
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
    _spinBoxZ.ValueChanged -= OnValueZChanged;
    _spinBoxW.ValueChanged -= OnValueWChanged;
  }

  public override void UpdateValue(object value)
  {
    Quaternion vector = (Quaternion)value;
    _spinBoxX.Value = vector.X;
    _spinBoxY.Value = vector.Y;
    _spinBoxZ.Value = vector.Z;
    _spinBoxW.Value = vector.W;
  }

  private void OnValueXChanged(double x)
  {
    Quaternion vector = ComponentInfo.GetFieldValue<Quaternion>(FieldName);
    vector.X = (float)x;
    ComponentInfo.SetFieldValue(FieldName, vector);
  }
  
  private void OnValueYChanged(double y)
  {
    Quaternion vector = ComponentInfo.GetFieldValue<Quaternion>(FieldName);
    vector.Y = (float)y;
    ComponentInfo.SetFieldValue(FieldName, vector);
  }
  
  private void OnValueZChanged(double z)
  {
    Quaternion vector = ComponentInfo.GetFieldValue<Quaternion>(FieldName);
    vector.Z = (float)z;
    ComponentInfo.SetFieldValue(FieldName, vector);
  } 
  
  private void OnValueWChanged(double w)
  {
    Quaternion vector = ComponentInfo.GetFieldValue<Quaternion>(FieldName);
    vector.W = (float)w;
    ComponentInfo.SetFieldValue(FieldName, vector);
  }
}