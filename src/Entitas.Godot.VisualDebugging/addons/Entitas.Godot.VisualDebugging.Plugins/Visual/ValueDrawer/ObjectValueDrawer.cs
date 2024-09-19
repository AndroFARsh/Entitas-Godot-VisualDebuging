using Godot;

namespace Entitas.Godot;

public partial class ObjectValueDrawer : BaseValueDrawer
{
  private Label _label;

  protected override void InitializeDrawer()
  {
    _label ??= CreateRow();
  }

  private Label CreateRow()
  {
    Label label = new();
    label.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    GridContainer.AddChild(label);
    return label;
  }

  protected override void CleanUpDrawer()
  {
  }

  public override void UpdateValue(object value) => _label.Text = value != null
    ? value.GetType().FullName
    : "NULL";
}