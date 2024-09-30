using Godot;

namespace Entitas.Godot;

public abstract partial class IntegerValueDrawer : BaseValueDrawer
{
  protected SpinBox _spinBox;
  
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
  
  protected abstract void OnValueChanged(double value);
}