using Godot;

namespace Entitas.Godot;

public partial class LongValueDrawer : BaseValueDrawer
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
    spinBox.Rounded = true;
    spinBox.Step = 1;
    GridContainer.AddChild(spinBox);
    return spinBox;
  }
  
  protected override void CleanUpDrawer()
  {
    _spinBox.ValueChanged -= OnValueChanged;
  }
  
  public override void UpdateValue(object value) => _spinBox.Value = (long)value;
  
  private void OnValueChanged(double value) => ComponentInfo.SetFieldValue(FieldName, (long)value);
}