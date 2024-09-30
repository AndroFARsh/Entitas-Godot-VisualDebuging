namespace Entitas.Godot;

public partial class UShortValueDrawer : IntegerValueDrawer
{
  public override void UpdateValue(object value) => _spinBox.Value = (ushort)value;
  
  protected override void OnValueChanged(double value) => ComponentInfo.SetFieldValue(FieldName, (ushort)value);
}