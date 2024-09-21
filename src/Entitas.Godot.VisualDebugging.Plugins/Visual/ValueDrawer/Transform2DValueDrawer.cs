using Godot;

namespace Entitas.Godot;

public partial class Transform2DValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxRowXX;
  private SpinBox _spinBoxRowXY;
  private SpinBox _spinBoxRowXO;
  private SpinBox _spinBoxRowYX;
  private SpinBox _spinBoxRowYY;
  private SpinBox _spinBoxRowYO;
  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 3;

    _spinBoxRowXX ??= CreateRow("xx");
    _spinBoxRowXY ??= CreateRow("xy");
    _spinBoxRowXO ??= CreateRow("xo");
    _spinBoxRowYX ??= CreateRow("yx");
    _spinBoxRowYY ??= CreateRow("yx");
    _spinBoxRowYO ??= CreateRow("yo");
    
    _spinBoxRowXX.ValueChanged += OnValueXXChanged;
    _spinBoxRowXY.ValueChanged += OnValueXYChanged;
    _spinBoxRowXO.ValueChanged += OnValueXOChanged;
    
    _spinBoxRowYX.ValueChanged += OnValueYXChanged;
    _spinBoxRowYY.ValueChanged += OnValueYYChanged;
    _spinBoxRowYO.ValueChanged += OnValueYOChanged;
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
    _spinBoxRowXX.ValueChanged -= OnValueXXChanged;
    _spinBoxRowXY.ValueChanged -= OnValueXYChanged;
    _spinBoxRowXO.ValueChanged -= OnValueXOChanged;
    
    _spinBoxRowYX.ValueChanged -= OnValueYXChanged;
    _spinBoxRowYY.ValueChanged -= OnValueYYChanged;
    _spinBoxRowYO.ValueChanged -= OnValueYOChanged;
  }

  public override void UpdateValue(object value)
  {
    Transform2D tr = (Transform2D)value;
    _spinBoxRowXX.Value = tr.X.X;
    _spinBoxRowXY.Value = tr.X.Y;
    _spinBoxRowXO.Value = tr.Origin.X;
    
    _spinBoxRowYX.Value = tr.Y.X;
    _spinBoxRowYY.Value = tr.Y.Y;
    _spinBoxRowYO.Value = tr.Origin.Y;
  }

  private void OnValueXXChanged(double xx)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.X.X = (float)xx;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  private void OnValueXYChanged(double xy)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.X.Y = (float)xy;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  private void OnValueYXChanged(double yx)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.Y.X = (float)yx;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  private void OnValueYYChanged(double yy)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.Y.Y = (float)yy;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  private void OnValueXOChanged(double xo)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.Origin.X = (float)xo;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  private void OnValueYOChanged(double yo)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.Origin.Y = (float)yo;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
}