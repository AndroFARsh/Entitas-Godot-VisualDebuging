using Godot;

namespace Entitas.Godot;

public partial class ColorValueDrawer : BaseValueDrawer
{
  private ColorPickerButton _colorPicker;
  
  protected override void InitializeDrawer()
  {
    _colorPicker ??= CreateRow();
    _colorPicker.ColorChanged += OnColorChanged;
  }

  private ColorPickerButton CreateRow()
  {
    ColorPickerButton colorPicker = new();
    colorPicker.SizeFlagsVertical = SizeFlags.ExpandFill;
    colorPicker.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    GridContainer.AddChild(colorPicker);
    return colorPicker;
  }

  protected override void CleanUpDrawer()
  {
    _colorPicker.ColorChanged -= OnColorChanged;
  }

  public override void UpdateValue(object value) => _colorPicker.Color = (Color)value;
  
  private void OnColorChanged(Color value) => ComponentInfo.SetFieldValue(FieldName, value);
}