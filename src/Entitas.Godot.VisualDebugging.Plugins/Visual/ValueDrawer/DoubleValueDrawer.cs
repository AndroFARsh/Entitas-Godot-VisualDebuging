using Godot;

namespace Entitas.Godot;

public partial class DoubleValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBox;
  
  protected override void InitializeDrawer()
  {
    _spinBox ??= CreateRow();
    _spinBox.ValueChanged += OnValueChanged;
  }

  private SpinBox CreateRow()
  {
    SpinBox spinBox = new();
    spinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    spinBox.AllowGreater = true;
    spinBox.AllowLesser = true;
    spinBox.Rounded = false;
    spinBox.Step = 0.0001;
    GridContainer.AddChild(spinBox);
    return spinBox;
  }
  
  protected override void CleanUpDrawer()
  {
    _spinBox.ValueChanged -= OnValueChanged;
  }

  public override void UpdateValue(object value) => _spinBox.Value = (double)value;
  
  private void OnValueChanged(double value) => ComponentInfo.SetFieldValue(FieldName, value);
}