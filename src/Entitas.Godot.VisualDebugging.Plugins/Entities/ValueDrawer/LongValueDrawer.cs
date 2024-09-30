namespace Entitas.Godot;

public partial class LongValueDrawer : IntegerValueDrawer
{
  public override void UpdateValue(object value) => _spinBox.Value = (long)value;
  protected override void OnValueChanged(double value) => ComponentInfo.SetFieldValue(FieldName, (long)value);
}