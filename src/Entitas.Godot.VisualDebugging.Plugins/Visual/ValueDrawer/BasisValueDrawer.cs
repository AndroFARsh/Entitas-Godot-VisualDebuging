using Godot;

namespace Entitas.Godot;

public partial class BasisValueDrawer : BaseValueDrawer
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
    spinBox.Step = 0.1;
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
  }

  public override void UpdateValue(object value)
  {
    Basis basis = (Basis)value;

    _spinBoxRow1X.Value = basis.Row0.X;
    _spinBoxRow1Y.Value = basis.Row0.Y;
    _spinBoxRow1Z.Value = basis.Row0.Z;

    _spinBoxRow2X.Value = basis.Row1.X;
    _spinBoxRow2Y.Value = basis.Row1.Y;
    _spinBoxRow2Z.Value = basis.Row1.Z;

    _spinBoxRow3X.Value = basis.Row2.X;
    _spinBoxRow3Y.Value = basis.Row2.Y;
    _spinBoxRow3Z.Value = basis.Row2.Z;
  }

  private void OnValueRow3ZChanged(double value)
  {
    Basis basis = ComponentInfo.GetFieldValue<Basis>(FieldName);
    basis.Row2.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, basis);
  }

  private void OnValueRow3YChanged(double value)
  {
    Basis basis = ComponentInfo.GetFieldValue<Basis>(FieldName);
    basis.Row2.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, basis);
  }

  private void OnValueRow3XChanged(double value)
  {
    Basis basis = ComponentInfo.GetFieldValue<Basis>(FieldName);
    basis.Row2.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, basis);
  }

  private void OnValueRow2ZChanged(double value)
  {
    Basis basis = ComponentInfo.GetFieldValue<Basis>(FieldName);
    basis.Row1.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, basis);
  }

  private void OnValueRow2YChanged(double value)
  {
    Basis basis = ComponentInfo.GetFieldValue<Basis>(FieldName);
    basis.Row1.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, basis);
  }

  private void OnValueRow2XChanged(double value)
  {
    Basis basis = ComponentInfo.GetFieldValue<Basis>(FieldName);
    basis.Row1.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, basis);
  }

  private void OnValueRow1ZChanged(double value)
  {
    Basis basis = ComponentInfo.GetFieldValue<Basis>(FieldName);
    basis.Row0.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, basis);
  }

  private void OnValueRow1YChanged(double value)
  {
    Basis basis = ComponentInfo.GetFieldValue<Basis>(FieldName);
    basis.Row0.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, basis);
  }

  private void OnValueRow1XChanged(double value)
  {
    Basis basis = ComponentInfo.GetFieldValue<Basis>(FieldName);
    basis.Row0.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, basis);
  }
}