namespace Entitas.Godot;

public partial class ULongValueDrawer : IntegerValueDrawer
{
  public override void UpdateValue(object value) => _spinBox.Value = (ulong)value;
  
  protected override void OnValueChanged(double value) => ComponentInfo.SetFieldValue(FieldName, (ulong)value);
}