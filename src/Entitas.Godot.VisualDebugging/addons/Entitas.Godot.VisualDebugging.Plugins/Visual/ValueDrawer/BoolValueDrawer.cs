using Godot;

namespace Entitas.Godot;

public partial class BoolValueDrawer : BaseValueDrawer
{
  private CheckBox _checkBox;

  protected override void InitializeDrawer()
  {
    _checkBox ??= CreateRow();
    _checkBox.Pressed += OnCheckBoxPressed;
  }

  private CheckBox CreateRow()
  {
    CheckBox checkBox = new();
    checkBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    GridContainer.AddChild(checkBox);
    return checkBox;
  }
  
  protected override void CleanUpDrawer()
  {
    _checkBox.Pressed -= OnCheckBoxPressed;
  }
  
  public override void UpdateValue(object value) => _checkBox.ButtonPressed = (bool)value;
  
  private void OnCheckBoxPressed() => ComponentInfo.SetFieldValue(FieldName, _checkBox.Flat);
}