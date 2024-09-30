using Godot;

namespace Entitas.Godot;

public partial class PlaneValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxX;
  private SpinBox _spinBoxY;
  private SpinBox _spinBoxZ;
  private SpinBox _spinBoxD;
  
  private SpinBox _spinBoxNormalX;
  private SpinBox _spinBoxNormalY;
  private SpinBox _spinBoxNormalZ;
  
  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 4;

    _spinBoxX ??= CreateRow("x");
    _spinBoxY ??= CreateRow("y");
    _spinBoxZ ??= CreateRow("z");
    _spinBoxD ??= CreateRow("d");
    
    _spinBoxX.ValueChanged += OnValueXChanged;
    _spinBoxY.ValueChanged += OnValueYChanged;
    _spinBoxZ.ValueChanged += OnValueZChanged;
    _spinBoxD.ValueChanged += OnValueDChanged;

    bool shouldCreateNormal = _spinBoxNormalX == null || _spinBoxNormalY == null || _spinBoxNormalZ == null;
    if (shouldCreateNormal) CreateLabel("Normal");
    
    _spinBoxNormalX ??= CreateRow("x");
    _spinBoxNormalY ??= CreateRow("y");
    _spinBoxNormalZ ??= CreateRow("z");
    
    _spinBoxNormalX.ValueChanged += OnValueNormalXChanged;
    _spinBoxNormalY.ValueChanged += OnValueNormalYChanged;
    _spinBoxNormalZ.ValueChanged += OnValueNormalZChanged;
  }

  private Label CreateLabel(string title)
  {
    Label label = new();
    label.Text = title;
    GridContainer.AddChild(label);
    return label;
  }

  private SpinBox CreateRow(string title)
  {
    SpinBox spinBox = new();
    spinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    spinBox.Prefix = title;
    spinBox.AllowGreater = true;
    spinBox.AllowLesser = true;
    spinBox.Rounded = false;
    spinBox.Step = 0.0001;
    GridContainer.AddChild(spinBox);
    
    return spinBox;
  }
  
  protected override void CleanUpDrawer()
  {
    _spinBoxX.ValueChanged -= OnValueXChanged;
    _spinBoxY.ValueChanged -= OnValueYChanged;
    _spinBoxZ.ValueChanged -= OnValueZChanged;
    _spinBoxD.ValueChanged -= OnValueDChanged;
    
    _spinBoxNormalX.ValueChanged -= OnValueNormalXChanged;
    _spinBoxNormalY.ValueChanged -= OnValueNormalYChanged;
    _spinBoxNormalZ.ValueChanged -= OnValueNormalZChanged;
  }

  public override void UpdateValue(object value)
  {
    Plane plane = (Plane)value;
    
    _spinBoxX.Value = plane.X;
    _spinBoxY.Value = plane.Y;
    _spinBoxZ.Value = plane.Z;
    _spinBoxD.Value = plane.D;
    
    _spinBoxNormalX.Value = plane.Normal.X;
    _spinBoxNormalY.Value = plane.Normal.Y;
    _spinBoxNormalZ.Value = plane.Normal.Z;
  }

  private void OnValueXChanged(double x)
  {
    Plane plane = ComponentInfo.GetFieldValue<Plane>(FieldName);
    plane.X = (float)x;
    ComponentInfo.SetFieldValue(FieldName, plane);
  }
  
  private void OnValueYChanged(double y)
  {
    Plane plane = ComponentInfo.GetFieldValue<Plane>(FieldName);
    plane.Y = (float)y;
    ComponentInfo.SetFieldValue(FieldName, plane);
  }
  
  private void OnValueZChanged(double z)
  {
    Plane plane = ComponentInfo.GetFieldValue<Plane>(FieldName);
    plane.Z = (float)z;
    ComponentInfo.SetFieldValue(FieldName, plane);
  } 
  
  private void OnValueDChanged(double d)
  {
    Plane plane = ComponentInfo.GetFieldValue<Plane>(FieldName);
    plane.D = (float)d;
    ComponentInfo.SetFieldValue(FieldName, plane);
  }
  
  private void OnValueNormalXChanged(double x)
  {
    Plane plane = ComponentInfo.GetFieldValue<Plane>(FieldName);
    Vector3 normal = plane.Normal;
    normal.X = (float)x;
    plane.Normal = normal;
    ComponentInfo.SetFieldValue(FieldName, plane);
  }
  
  private void OnValueNormalYChanged(double y)
  {
    Plane plane = ComponentInfo.GetFieldValue<Plane>(FieldName);
    Vector3 normal = plane.Normal;
    normal.Y = (float)y;
    plane.Normal = normal;
    ComponentInfo.SetFieldValue(FieldName, plane);
  }
  
  private void OnValueNormalZChanged(double z)
  {
    Plane plane = ComponentInfo.GetFieldValue<Plane>(FieldName);
    Vector3 normal = plane.Normal;
    normal.Z = (float)z;
    plane.Normal = normal;
    ComponentInfo.SetFieldValue(FieldName, plane);
  }
}