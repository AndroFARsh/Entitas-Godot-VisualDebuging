using Godot;

namespace Entitas.Godot;

public partial class StringNameValueDrawer : BaseValueDrawer
{
  private TextEdit _textEdit;

  protected override void InitializeDrawer()
  {
    _textEdit ??= CreateRow();
    _textEdit.TextChanged += OnTextChanged;
  }

  private TextEdit CreateRow()
  {
    TextEdit textEdit = new();
    textEdit.SetFitContentHeightEnabled(true);
    textEdit.SizeFlagsVertical = SizeFlags.ExpandFill;
    textEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    GridContainer.AddChild(textEdit);
    return textEdit;
  }

  protected override void CleanUpDrawer()
  {
    _textEdit.TextChanged -= OnTextChanged;
  }

  public override void UpdateValue(object value) => _textEdit.Text = (StringName)value;

  private void OnTextChanged() => ComponentInfo.SetFieldValue(FieldName, (StringName)_textEdit.Text);
}