using Godot;

namespace Entitas.Godot;

public partial class Transform3DValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxRow1X;
  private SpinBox _spinBoxRow1Y;
  private SpinBox _spinBoxRow1Z;

  private SpinBox _spinBoxRow2X;
  private SpinBox _spinBoxRow2Y;
  private SpinBox _spinBoxRow2Z;

  private SpinBox _spinBoxRow3X;
  private SpinBox _spinBoxRow3Y;
  private SpinBox _spinBoxRow3Z;
  
  private SpinBox _spinBoxOriginX;
  private SpinBox _spinBoxOriginY;
  private SpinBox _spinBoxOriginZ;

  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 3;
    if (_spinBoxRow1X == null)
    {
      CreateLabel("Basis", new Color(1f, 1f, 1f));
      CreateDummy();
      CreateDummy();
    }

    _spinBoxRow1X ??= CreateRow();
    _spinBoxRow1Y ??= CreateRow();
    _spinBoxRow1Z ??= CreateRow();

    _spinBoxRow2X ??= CreateRow();
    _spinBoxRow2Y ??= CreateRow();
    _spinBoxRow2Z ??= CreateRow();

    _spinBoxRow3X ??= CreateRow();
    _spinBoxRow3Y ??= CreateRow();
    _spinBoxRow3Z ??= CreateRow();

    _spinBoxRow1X.ValueChanged += OnValueRow1XChanged;
    _spinBoxRow1Y.ValueChanged += OnValueRow1YChanged;
    _spinBoxRow1Z.ValueChanged += OnValueRow1ZChanged;

    _spinBoxRow2X.ValueChanged += OnValueRow2XChanged;
    _spinBoxRow2Y.ValueChanged += OnValueRow2YChanged;
    _spinBoxRow2Z.ValueChanged += OnValueRow2ZChanged;

    _spinBoxRow3X.ValueChanged += OnValueRow3XChanged;
    _spinBoxRow3Y.ValueChanged += OnValueRow3YChanged;
    _spinBoxRow3Z.ValueChanged += OnValueRow3ZChanged;
    
    if (_spinBoxRow1X == null)
    {
      CreateLabel("Origin", new Color(1f, 1f, 1f));
      CreateDummy();
      CreateDummy();
    }
    
    _spinBoxOriginX ??= CreateRow();
    _spinBoxOriginY ??= CreateRow();
    _spinBoxOriginZ ??= CreateRow();
    
    _spinBoxOriginX.ValueChanged += OnValueOriginXChanged;
    _spinBoxOriginY.ValueChanged += OnValueOriginYChanged;
    _spinBoxOriginZ.ValueChanged += OnValueOriginZChanged;
  }

  private Control CreateDummy()
  {
    Control control = new();
    control.Size = new Vector2(0, 0);
    GridContainer.AddChild(control);
    return control;
  }

  private Label CreateLabel(string title, Color titleColor)
  {
    Label label = new();
    label.Text = title;
    label.Set("theme_override_colors/font_color", titleColor);
    GridContainer.AddChild(label);
    return label;
  }

  private SpinBox CreateRow()
  {
    SpinBox spinBox = new();
    spinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    spinBox.AllowGreater = true;
    spinBox.AllowLesser = true;
    spinBox.Rounded = false;
    spinBox.Step = 0.0001;
    GridContainer.AddChild(spinBox);

    return spinBox;
  }

  protected override void CleanUpDrawer()
  {
    _spinBoxRow1X.ValueChanged -= OnValueRow1XChanged;
    _spinBoxRow1Y.ValueChanged -= OnValueRow1YChanged;
    _spinBoxRow1Z.ValueChanged -= OnValueRow1ZChanged;

    _spinBoxRow2X.ValueChanged -= OnValueRow2XChanged;
    _spinBoxRow2Y.ValueChanged -= OnValueRow2YChanged;
    _spinBoxRow2Z.ValueChanged -= OnValueRow2ZChanged;

    _spinBoxRow3X.ValueChanged -= OnValueRow3XChanged;
    _spinBoxRow3Y.ValueChanged -= OnValueRow3YChanged;
    _spinBoxRow3Z.ValueChanged -= OnValueRow3ZChanged;
    
    _spinBoxOriginX.ValueChanged -= OnValueOriginXChanged;
    _spinBoxOriginY.ValueChanged -= OnValueOriginYChanged;
    _spinBoxOriginZ.ValueChanged -= OnValueOriginZChanged;
  }

  public override void UpdateValue(object value)
  {
    Transform3D tr = (Transform3D)value;

    _spinBoxRow1X.Value = tr.Basis.Row0.X;
    _spinBoxRow1Y.Value = tr.Basis.Row0.Y;
    _spinBoxRow1Z.Value = tr.Basis.Row0.Z;

    _spinBoxRow2X.Value = tr.Basis.Row1.X;
    _spinBoxRow2Y.Value = tr.Basis.Row1.Y;
    _spinBoxRow2Z.Value = tr.Basis.Row1.Z;

    _spinBoxRow3X.Value = tr.Basis.Row2.X;
    _spinBoxRow3Y.Value = tr.Basis.Row2.Y;
    _spinBoxRow3Z.Value = tr.Basis.Row2.Z;
  }

  private void OnValueRow3ZChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Basis.Row2.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }

  private void OnValueRow3YChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Basis.Row2.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }

  private void OnValueRow3XChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Basis.Row2.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }

  private void OnValueRow2ZChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Basis.Row1.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }

  private void OnValueRow2YChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Basis.Row1.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }

  private void OnValueRow2XChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Basis.Row1.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }

  private void OnValueRow1ZChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Basis.Row0.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }

  private void OnValueRow1YChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Basis.Row0.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }

  private void OnValueRow1XChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Basis.Row0.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
  
  
  private void OnValueOriginZChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Origin.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }

  private void OnValueOriginYChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Origin.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }

  private void OnValueOriginXChanged(double value)
  {
    Transform3D tr = ComponentInfo.GetFieldValue<Transform3D>(FieldName);
    tr.Origin.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, tr);
  }
}