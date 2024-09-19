using Godot;

namespace Entitas.Godot;

public partial class AabbValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxX;
  private SpinBox _spinBoxY;
  private SpinBox _spinBoxZ;
  private SpinBox _spinBoxW;
  private SpinBox _spinBoxH;
  private SpinBox _spinBoxD;
  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 3;

    _spinBoxX ??= CreateRow("x");
    _spinBoxY ??= CreateRow("y");
    _spinBoxZ ??= CreateRow("z");
    
    _spinBoxW ??= CreateRow("w");
    _spinBoxH ??= CreateRow("h");
    _spinBoxD ??= CreateRow("d");
    
    _spinBoxX.ValueChanged += OnValueXChanged;
    _spinBoxY.ValueChanged += OnValueYChanged;
    _spinBoxZ.ValueChanged += OnValueZChanged;
    
    _spinBoxW.ValueChanged += OnValueWChanged;
    _spinBoxH.ValueChanged += OnValueHChanged;
    _spinBoxD.ValueChanged += OnValueDChanged;
  }

  private SpinBox CreateRow(string title)
  {
    SpinBox spinBox = new();
    spinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    spinBox.Prefix = title;
    spinBox.AllowGreater = true;
    spinBox.AllowLesser = true;
    spinBox.Rounded = true;
    spinBox.Step = 1;
    GridContainer.AddChild(spinBox);
    
    return spinBox;
  }

  protected override void CleanUpDrawer()
  {
    _spinBoxX.ValueChanged -= OnValueXChanged;
    _spinBoxY.ValueChanged -= OnValueYChanged;
    _spinBoxZ.ValueChanged -= OnValueZChanged;
    
    _spinBoxW.ValueChanged -= OnValueWChanged;
    _spinBoxH.ValueChanged -= OnValueHChanged;
    _spinBoxD.ValueChanged -= OnValueDChanged;
  }

  public override void UpdateValue(object value)
  {
    Aabb aabb = (Aabb)value;
    _spinBoxX.Value = aabb.Position.X;
    _spinBoxY.Value = aabb.Position.Y;
    _spinBoxZ.Value = aabb.Position.Z;
    _spinBoxW.Value = aabb.Size.X;
    _spinBoxH.Value = aabb.Size.Y;
    _spinBoxD.Value = aabb.Size.Z;
  }

  private void OnValueXChanged(double x)
  {
    Aabb aabb = ComponentInfo.GetFieldValue<Aabb>(FieldName);
    Vector3 position = aabb.Position;
    position.X = (int)x;
    aabb.Position = position;
    ComponentInfo.SetFieldValue(FieldName, aabb);
  }
  
  private void OnValueYChanged(double y)
  {
    Aabb aabb = ComponentInfo.GetFieldValue<Aabb>(FieldName);
    Vector3 position = aabb.Position;
    position.Y = (int)y;
    aabb.Position = position;
    ComponentInfo.SetFieldValue(FieldName, aabb);
  } 
  
  private void OnValueZChanged(double z)
  {
    Aabb aabb = ComponentInfo.GetFieldValue<Aabb>(FieldName);
    Vector3 position = aabb.Position;
    position.Z = (int)z;
    aabb.Position = position;
    ComponentInfo.SetFieldValue(FieldName, aabb);
  } 
  
  private void OnValueWChanged(double w)
  {
    Aabb aabb = ComponentInfo.GetFieldValue<Aabb>(FieldName);
    Vector3 size = aabb.Size;
    size.X = (int)w;
    aabb.Size = size;
    ComponentInfo.SetFieldValue(FieldName, aabb);
  }
  
  private void OnValueHChanged(double h)
  {
    Aabb aabb = ComponentInfo.GetFieldValue<Aabb>(FieldName);
    Vector3 size = aabb.Size;
    size.Y = (int)h;
    aabb.Size = size;
    ComponentInfo.SetFieldValue(FieldName, aabb);
  }
  
  private void OnValueDChanged(double d)
  {
    Aabb aabb = ComponentInfo.GetFieldValue<Aabb>(FieldName);
    Vector3 size = aabb.Size;
    size.Z = (int)d;
    aabb.Size = size;
    ComponentInfo.SetFieldValue(FieldName, aabb);
  }
}