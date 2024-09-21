using Godot;

namespace Entitas.Godot;

public abstract partial class BaseValueDrawer : MarginContainer
{
  private string _fieldName;
  private ComponentInfo _componentInfo;
  
  private ColorRect _bg;
  private Label _title;
  private GridContainer _gridContainer;

  public string FieldName => _fieldName;

  public ComponentInfo ComponentInfo => _componentInfo;

  public GridContainer GridContainer => _gridContainer;
  
  public void Initialize(ComponentInfo componentInfo, string fieldName,  bool showBg)
  {
    _componentInfo = componentInfo;
    _fieldName = fieldName;

    InitializeTree();
    
    _bg.Visible = showBg;
    _title.Text = fieldName;
    
    SizeFlagsHorizontal = SizeFlags.ExpandFill;
    InitializeDrawer();
    UpdateValue(_componentInfo.GetFieldValue(_fieldName));
  }

  private void InitializeTree()
  {
    if (GetChildCount() > 0) return;
    
    AnchorsPreset = (int) LayoutPreset.FullRect;
    GrowHorizontal = GrowDirection.Both;
    GrowVertical = GrowDirection.Both;
    SizeFlagsHorizontal = SizeFlags.ExpandFill;

    _bg = new ColorRect();
    _bg.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    _bg.SizeFlagsVertical = SizeFlags.ExpandFill;
    _bg.Color = new Color(1, 1, 1, 0.1f);
    AddChild(_bg);

    MarginContainer marginContainer = new();
    marginContainer.AddThemeConstantOverride("margin_top", Consts.Margin);
    marginContainer.AddThemeConstantOverride("margin_left", Consts.Margin);
    marginContainer.AddThemeConstantOverride("margin_bottom", Consts.Margin);
    marginContainer.AddThemeConstantOverride("margin_right", Consts.Margin);
    AddChild(marginContainer);

    HBoxContainer rowContainer = new HBoxContainer();
    rowContainer.AddThemeConstantOverride("separation", Consts.Margin);
    rowContainer.SizeFlagsVertical = SizeFlags.ShrinkBegin;
    marginContainer.AddChild(rowContainer);

    _title = new Label();
    _title.SizeFlagsVertical = SizeFlags.ShrinkBegin;
    rowContainer.AddChild(_title);
    
    _gridContainer = new GridContainer();
    _gridContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    rowContainer.AddChild(_gridContainer);
  }

  public void CleanUp()
  {
    CleanUpDrawer();
    
    _componentInfo = null;
    _fieldName = null;
  }

  protected abstract void CleanUpDrawer();
  
  protected abstract void InitializeDrawer();

  public abstract void UpdateValue(object value);
}