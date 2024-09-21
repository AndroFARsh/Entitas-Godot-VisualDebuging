using System;
using Godot;

namespace Entitas.Godot;

public partial class SystemInspector : BaseInspector
{
  class SystemExecTimeRow : IDisposable
  {
    private Label _title;
    private SpinBox _value;
    
    public SystemExecTimeRow(
      string title, 
      Control parent
    ) {
      _title = new Label();
      _title.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
      _title.LayoutMode = 2;
      _title.Text = title;
      parent.AddChild(_title);

      _value = new SpinBox();
      _value.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
      _value.LayoutMode = 2;
      _value.Step = 0.0001;
      _value.AllowGreater = true;
      _value.AllowLesser = true;
      _value.Editable = false;
      parent.AddChild(_value);
    }

    public void UpdateValue(double value) => _value.Value = value;

    public void Dispose()
    {
      _title?.QueueFree();
      _value?.QueueFree();
      _title = null;
      _value = null;
    }
  }

  private CheckBox _activate;
  
  private SystemExecTimeRow _initializationRow;
  private SystemExecTimeRow _executeRow;
  private SystemExecTimeRow _cleanupRow;
  private SystemExecTimeRow _tearDownRow;
  
  private SystemInfo _systemInfo;
  private SystemObserverNode _systemObserverNode;
  
  public override void Initialize(Node node)
  {
    if (node == _systemObserverNode) return;
    _systemObserverNode = (SystemObserverNode)node;
    _systemInfo = _systemObserverNode.SystemInfo;

    AddThemeConstantOverride("margin_top", Consts.Margin);
    AddThemeConstantOverride("margin_left", Consts.Margin);
    AddThemeConstantOverride("margin_bottom", Consts.Margin);
    AddThemeConstantOverride("margin_right", Consts.Margin);
    LayoutMode =   2;
    AnchorsPreset = (int) LayoutPreset.FullRect;
    AnchorRight = 1;
    AnchorBottom = 1;
    GrowHorizontal = GrowDirection.Both;
    GrowVertical = GrowDirection.Both;
    SizeFlagsHorizontal = SizeFlags.ExpandFill;

    ColorRect colorRect = new();
    colorRect.Color = new Color(1, 0, 0);
    colorRect.SizeFlagsVertical = SizeFlags.ExpandFill;
    colorRect.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    AddChild(colorRect);
 
    GridContainer gridContainer = new();
    gridContainer.AddThemeConstantOverride("h_separation", Consts.Margin);
    gridContainer.AddThemeConstantOverride("v_separation", Consts.Margin);
    gridContainer.LayoutMode = 2;
    gridContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    gridContainer.GrowHorizontal = GrowDirection.Both;
    gridContainer.Columns = 2;
    AddChild(gridContainer);

    Label title = new();
    title.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    title.Text = _systemInfo.SystemName;
    title.LayoutMode = 2;
    gridContainer.AddChild(title);
    
    _activate = new CheckBox();
    _activate.LayoutMode = 2;
    _activate.ClipText = true;
    _activate.ButtonPressed = _systemInfo.IsActive;
    _activate.Pressed += OnActivationChanged;
    gridContainer.AddChild(_activate);

    if (_systemInfo.IsInitializeSystems) _initializationRow = new SystemExecTimeRow("Initialize Time:", gridContainer);
    if (_systemInfo.IsExecuteSystems) _executeRow = new SystemExecTimeRow("Execute Time:", gridContainer);
    if (_systemInfo.IsCleanupSystems) _cleanupRow = new SystemExecTimeRow("Cleanup Time:", gridContainer);
    if (_systemInfo.IsTearDownSystems) _tearDownRow = new SystemExecTimeRow("TearDown Time:", gridContainer);
    
    _systemInfo.InitializationDurationChanged += OnInitializationDurationChanged;
    _systemInfo.ExecuteDurationChanged += OnExecuteDurationChanged;
    _systemInfo.CleanupDurationChanged += OnCleanupDurationChanged;
    _systemInfo.TeardownDurationChanged += OnTeardownDurationChanged;
  }

  private void OnTeardownDurationChanged() => _tearDownRow?.UpdateValue(_systemInfo.TeardownDuration);
  
  private void OnCleanupDurationChanged() => _cleanupRow?.UpdateValue(_systemInfo.AverageCleanupDuration);
  
  private void OnExecuteDurationChanged() => _executeRow?.UpdateValue(_systemInfo.AverageExecutionDuration);

  private void OnInitializationDurationChanged() => _initializationRow?.UpdateValue(_systemInfo.InitializationDuration);

  private void OnActivationChanged() => _systemInfo.IsActive = _activate.Flat;

  public override void CleanUp()
  {
    if (_systemObserverNode == null) return;

    _systemInfo.InitializationDurationChanged -= OnInitializationDurationChanged;
    _systemInfo.ExecuteDurationChanged -= OnExecuteDurationChanged;
    _systemInfo.CleanupDurationChanged -= OnCleanupDurationChanged;
    _systemInfo.TeardownDurationChanged -= OnTeardownDurationChanged;
    
    _activate.Pressed -= OnActivationChanged;
    
    foreach (Node child in GetChildren())
      child.QueueFree();
    
    _initializationRow?.Dispose();
    _executeRow?.Dispose();
    _cleanupRow?.Dispose();
    _tearDownRow?.Dispose();

    _initializationRow = null;
    _executeRow = null;
    _cleanupRow = null;
    _tearDownRow = null;
    
    _systemObserverNode = null;
    _systemInfo = null;
  }
}