namespace Entitas.Godot;

public partial class ShortValueDrawer : IntegerValueDrawer
{
  public override void UpdateValue(object value) => _spinBox.Value = (short)value;
  protected override void OnValueChanged(double value) => ComponentInfo.SetFieldValue(FieldName, (short)value);
}