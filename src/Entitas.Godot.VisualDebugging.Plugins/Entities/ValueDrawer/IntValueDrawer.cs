namespace Entitas.Godot;

public partial class IntValueDrawer : IntegerValueDrawer
{
  public override void UpdateValue(object value) => _spinBox.Value = (int)value;
  
  protected override void OnValueChanged(double value) => ComponentInfo.SetFieldValue(FieldName, (int)value);
}