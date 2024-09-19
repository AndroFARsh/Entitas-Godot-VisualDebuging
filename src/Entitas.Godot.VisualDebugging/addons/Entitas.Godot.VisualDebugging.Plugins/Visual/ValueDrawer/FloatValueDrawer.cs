using Godot;

namespace Entitas.Godot;

public partial class FloatValueDrawer : BaseValueDrawer
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
    spinBox.Step = 0.1;
    GridContainer.AddChild(spinBox);
    return spinBox;
  }
  
  protected override void CleanUpDrawer()
  {
    _spinBox.ValueChanged -= OnValueChanged;
  }

  public override void UpdateValue(object value) => _spinBox.Value = (float)value;
  
  private void OnValueChanged(double value) => ComponentInfo.SetFieldValue(FieldName, (float)value);
}