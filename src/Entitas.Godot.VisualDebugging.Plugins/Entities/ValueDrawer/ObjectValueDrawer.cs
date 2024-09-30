using Godot;

namespace Entitas.Godot;

public partial class ObjectValueDrawer : BaseValueDrawer
{
  private TextEdit _textEdit;

  protected override void InitializeDrawer()
  {
    _textEdit ??= CreateRow();
  }

  private TextEdit CreateRow()
  {
    TextEdit textEdit = new();
    textEdit.SizeFlagsVertical = SizeFlags.ExpandFill;
    textEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    textEdit.Editable = false;
    textEdit.ScrollFitContentHeight = true;
    GridContainer.AddChild(textEdit);
    return textEdit;
  }

  protected override void CleanUpDrawer()
  {
  }

  public override void UpdateValue(object value) => _textEdit.Text = value != null
    ? value.GetType().FullName
    : "NULL";
}