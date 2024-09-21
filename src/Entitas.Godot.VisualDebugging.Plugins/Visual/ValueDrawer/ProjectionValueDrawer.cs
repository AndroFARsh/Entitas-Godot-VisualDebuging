using Godot;

namespace Entitas.Godot;

public partial class ProjectionValueDrawer : BaseValueDrawer
{
  private SpinBox _spinBoxRow1X;
  private SpinBox _spinBoxRow1Y;
  private SpinBox _spinBoxRow1Z;
  private SpinBox _spinBoxRow1W;

  private SpinBox _spinBoxRow2X;
  private SpinBox _spinBoxRow2Y;
  private SpinBox _spinBoxRow2Z;
  private SpinBox _spinBoxRow2W;

  private SpinBox _spinBoxRow3X;
  private SpinBox _spinBoxRow3Y;
  private SpinBox _spinBoxRow3Z;
  private SpinBox _spinBoxRow3W;

  private SpinBox _spinBoxRow4X;
  private SpinBox _spinBoxRow4Y;
  private SpinBox _spinBoxRow4Z;
  private SpinBox _spinBoxRow4W;
  
  protected override void InitializeDrawer()
  {
    GridContainer.Columns = 4;
    if (_spinBoxRow1X == null)
    {
      CreateLabel("Projection", new Color(1f, 1f, 1f));
      CreateDummy();
      CreateDummy();
      CreateDummy();
    }

    _spinBoxRow1X ??= CreateRow();
    _spinBoxRow1Y ??= CreateRow();
    _spinBoxRow1Z ??= CreateRow();
    _spinBoxRow1W ??= CreateRow();

    _spinBoxRow2X ??= CreateRow();
    _spinBoxRow2Y ??= CreateRow();
    _spinBoxRow2Z ??= CreateRow();
    _spinBoxRow2W ??= CreateRow();

    _spinBoxRow3X ??= CreateRow();
    _spinBoxRow3Y ??= CreateRow();
    _spinBoxRow3Z ??= CreateRow();
    _spinBoxRow3W ??= CreateRow();

    _spinBoxRow4X ??= CreateRow();
    _spinBoxRow4Y ??= CreateRow();
    _spinBoxRow4Z ??= CreateRow();
    _spinBoxRow4W ??= CreateRow();
    
    _spinBoxRow1X.ValueChanged += OnValueRow1XChanged;
    _spinBoxRow1Y.ValueChanged += OnValueRow1YChanged;
    _spinBoxRow1Z.ValueChanged += OnValueRow1ZChanged;
    _spinBoxRow1W.ValueChanged += OnValueRow1WChanged;

    _spinBoxRow2X.ValueChanged += OnValueRow2XChanged;
    _spinBoxRow2Y.ValueChanged += OnValueRow2YChanged;
    _spinBoxRow2Z.ValueChanged += OnValueRow2ZChanged;
    _spinBoxRow2W.ValueChanged += OnValueRow2WChanged;

    _spinBoxRow3X.ValueChanged += OnValueRow3XChanged;
    _spinBoxRow3Y.ValueChanged += OnValueRow3YChanged;
    _spinBoxRow3Z.ValueChanged += OnValueRow3ZChanged;
    _spinBoxRow3W.ValueChanged += OnValueRow3WChanged;
    
    _spinBoxRow4X.ValueChanged += OnValueRow4XChanged;
    _spinBoxRow4Y.ValueChanged += OnValueRow4YChanged;
    _spinBoxRow4Z.ValueChanged += OnValueRow4ZChanged;
    _spinBoxRow4W.ValueChanged += OnValueRow4WChanged;
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
    _spinBoxRow1W.ValueChanged -= OnValueRow1WChanged;

    _spinBoxRow2X.ValueChanged -= OnValueRow2XChanged;
    _spinBoxRow2Y.ValueChanged -= OnValueRow2YChanged;
    _spinBoxRow2Z.ValueChanged -= OnValueRow2ZChanged;
    _spinBoxRow2W.ValueChanged -= OnValueRow2WChanged;

    _spinBoxRow3X.ValueChanged -= OnValueRow3XChanged;
    _spinBoxRow3Y.ValueChanged -= OnValueRow3YChanged;
    _spinBoxRow3Z.ValueChanged -= OnValueRow3ZChanged;
    _spinBoxRow3W.ValueChanged -= OnValueRow3WChanged;
    
    _spinBoxRow4X.ValueChanged -= OnValueRow4XChanged;
    _spinBoxRow4Y.ValueChanged -= OnValueRow4YChanged;
    _spinBoxRow4Z.ValueChanged -= OnValueRow4ZChanged;
    _spinBoxRow4W.ValueChanged -= OnValueRow4WChanged;
  }

  public override void UpdateValue(object value)
  {
    Projection projection = (Projection)value;

    _spinBoxRow1X.Value = projection.X.X;
    _spinBoxRow1Y.Value = projection.X.Y;
    _spinBoxRow1Z.Value = projection.X.Z;
    _spinBoxRow1W.Value = projection.X.W;

    _spinBoxRow2X.Value = projection.Y.X;
    _spinBoxRow2Y.Value = projection.Y.Y;
    _spinBoxRow2Z.Value = projection.Y.Z;
    _spinBoxRow2W.Value = projection.Y.W;

    _spinBoxRow3X.Value = projection.Z.X;
    _spinBoxRow3Y.Value = projection.Z.Y;
    _spinBoxRow3Z.Value = projection.Z.Z;
    _spinBoxRow3W.Value = projection.Z.W;
    
    _spinBoxRow4X.Value = projection.W.X;
    _spinBoxRow4Y.Value = projection.W.Y;
    _spinBoxRow4Z.Value = projection.W.Z;
    _spinBoxRow4W.Value = projection.W.W;
  }

  private void OnValueRow4WChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.W.W = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }
  
  private void OnValueRow4ZChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.W.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }

  private void OnValueRow4YChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.W.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }

  private void OnValueRow4XChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.W.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }
  
  private void OnValueRow3WChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.Z.W = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }
  
  private void OnValueRow3ZChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.Z.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }

  private void OnValueRow3YChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.Z.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }

  private void OnValueRow3XChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.Z.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }

  private void OnValueRow2WChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.Y.W = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }
  
  private void OnValueRow2ZChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.Y.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }

  private void OnValueRow2YChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.Y.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }

  private void OnValueRow2XChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.Y.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }

  private void OnValueRow1WChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.X.W = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }
  
  private void OnValueRow1ZChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.X.Z = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }

  private void OnValueRow1YChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.X.Y = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }

  private void OnValueRow1XChanged(double value)
  {
    Projection projection = ComponentInfo.GetFieldValue<Projection>(FieldName);
    projection.X.X = (float)value;
    ComponentInfo.SetFieldValue(FieldName, projection);
  }
}