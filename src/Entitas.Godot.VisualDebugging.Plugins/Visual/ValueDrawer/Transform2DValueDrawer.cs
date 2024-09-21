using Godot;

namespace Entitas.Godot;

public partial class Transform2DValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxRow1X;
  private SpinBox _spinBoxRow1Y;
  private SpinBox _spinBoxRow2X;
  private SpinBox _spinBoxRow2Y;
  private SpinBox _spinBoxRow3X;
  private SpinBox _spinBoxRow3Y;
  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 3;

    _spinBoxRow1X ??= CreateRow("x1");
    _spinBoxRow1Y ??= CreateRow("y1");
    _spinBoxRow2X ??= CreateRow("x2");
    _spinBoxRow2Y ??= CreateRow("y2");
    _spinBoxRow3X ??= CreateRow("x3");
    _spinBoxRow3Y ??= CreateRow("y3");
    
    _spinBoxRow1X.ValueChanged += OnValue1XChanged;
    _spinBoxRow1Y.ValueChanged += OnValue1YChanged;
    _spinBoxRow2X.ValueChanged += OnValue2XChanged;
    _spinBoxRow2Y.ValueChanged += OnValue2YChanged;
    _spinBoxRow3X.ValueChanged += OnValue3XChanged;
    _spinBoxRow3Y.ValueChanged += OnValue3YChanged;
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
    _spinBoxRow1X.ValueChanged -= OnValue1XChanged;
    _spinBoxRow1Y.ValueChanged -= OnValue1YChanged;
    _spinBoxRow2X.ValueChanged -= OnValue2XChanged;
    _spinBoxRow2Y.ValueChanged -= OnValue2YChanged;
    _spinBoxRow3X.ValueChanged -= OnValue3XChanged;
    _spinBoxRow3Y.ValueChanged -= OnValue3YChanged;
  }

  public override void UpdateValue(object value)
  {
    Transform2D tr = (Transform2D)value;
    _spinBoxRow1X.Value = tr.X.X;
    _spinBoxRow1Y.Value = tr.X.Y;
    _spinBoxRow2X.Value = tr.Y.X;
    _spinBoxRow2Y.Value = tr.Y.Y;
    _spinBoxRow3X.Value = tr.Origin.X;
    _spinBoxRow3Y.Value = tr.Origin.Y;
  }

  private void OnValue1XChanged(double x1)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.X.X = (float)x1;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  private void OnValue1YChanged(double y1)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.X.Y = (float)y1;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  private void OnValue2XChanged(double x2)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.Y.X = (float)x2;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  private void OnValue2YChanged(double y2)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.Y.Y = (float)y2;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  private void OnValue3XChanged(double x3)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.Origin.X = (float)x3;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  private void OnValue3YChanged(double y3)
  {
    Transform2D tr = ComponentInfo.GetFieldValue<Transform2D>(FieldName);
    tr.Origin.Y = (float)y3;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
}