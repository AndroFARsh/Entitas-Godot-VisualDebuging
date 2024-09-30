using Godot;

namespace Entitas.Godot;

public partial class StringValueDrawer : BaseValueDrawer
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
    textEdit.ScrollFitContentHeight = true;
    GridContainer.AddChild(textEdit);
    return textEdit;
  }

  protected override void CleanUpDrawer()
  {
    _textEdit.TextChanged -= OnTextChanged;
  }
  
  public override void UpdateValue(object value) => _textEdit.Text = (string)value;
  
  private void OnTextChanged() => ComponentInfo.SetFieldValue(FieldName, _textEdit.Text);
}