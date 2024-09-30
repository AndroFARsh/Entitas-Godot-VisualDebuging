namespace Entitas.Godot;

public partial class UIntValueDrawer : IntegerValueDrawer
{
  public override void UpdateValue(object value) => _spinBox.Value = (uint)value;
  
  protected override void OnValueChanged(double value) => ComponentInfo.SetFieldValue(FieldName, (uint)value);
}