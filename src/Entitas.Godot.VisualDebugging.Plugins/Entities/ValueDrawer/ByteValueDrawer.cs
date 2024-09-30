namespace Entitas.Godot;

public partial class ByteValueDrawer : IntegerValueDrawer
{
  public override void UpdateValue(object value) => _spinBox.Value = (byte)value;
  
  protected override void OnValueChanged(double value) => ComponentInfo.SetFieldValue(FieldName, (byte)value);
}