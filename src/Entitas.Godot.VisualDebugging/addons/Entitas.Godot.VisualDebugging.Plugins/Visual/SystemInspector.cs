using Godot;

namespace Entitas.Godot;

public partial class SystemInspector : BaseInspector
{
  [Export] private Label _title;
  [Export] private CheckBox _activate;
  
  [Export] private Label _initializeTitle;
  [Export] private SpinBox _initializeValue;
  
  [Export] private Label _executeTitle;
  [Export] private SpinBox _executeValue;
  
  [Export] private Label _cleanupTitle;
  [Export] private SpinBox _cleanupValue;
  
  [Export] private Label _teardownTitle;
  [Export] private SpinBox _teardownValue;

  private SystemInfo _systemInfo;
  private SystemObserverNode _systemObserverNode;
  
  public override void Initialize(Node node)
  {
    if (node == _systemObserverNode) return;
    _systemObserverNode = (SystemObserverNode)node;
    _systemInfo = _systemObserverNode.SystemInfo;

    _title.Text = _systemInfo.SystemName;
    _activate.ButtonPressed = _systemInfo.IsActive;
    _activate.Pressed += OnActivationChanged;
    
    _initializeTitle.Visible = _systemInfo.IsInitializeSystems;
    _initializeValue.Visible = _systemInfo.IsInitializeSystems;
    _initializeValue.Suffix = "ms";
    _systemInfo.InitializationDurationChanged += OnInitializationDurationChanged;
      
    _executeTitle.Visible = _systemInfo.IsExecuteSystems;
    _executeValue.Visible = _systemInfo.IsExecuteSystems;
    _executeValue.Suffix = "ms";
    _systemInfo.ExecuteDurationChanged += OnExecuteDurationChanged;
    
    _cleanupTitle.Visible = _systemInfo.IsCleanupSystems;
    _cleanupValue.Visible = _systemInfo.IsCleanupSystems;
    _cleanupValue.Suffix = "ms";
    _systemInfo.CleanupDurationChanged += OnCleanupDurationChanged;
  
    _teardownTitle.Visible = _systemInfo.IsTearDownSystems;
    _teardownValue.Visible = _systemInfo.IsTearDownSystems;
    _teardownValue.Suffix = "ms";
    _systemInfo.TeardownDurationChanged += OnTeardownDurationChanged;
  }

  private void OnTeardownDurationChanged() => _teardownValue.Value = _systemInfo.TeardownDuration;

  private void OnCleanupDurationChanged() => _cleanupValue.Value = _systemInfo.AverageCleanupDuration;

  private void OnExecuteDurationChanged() => _executeValue.Value = _systemInfo.AverageExecutionDuration;

  private void OnInitializationDurationChanged() => _initializeValue.Value = _systemInfo.InitializationDuration;

  private void OnActivationChanged() => _systemInfo.IsActive = _activate.Flat;

  public override void CleanUp()
  {
    if (_systemObserverNode == null) return;
    
    _systemInfo.InitializationDurationChanged -= OnInitializationDurationChanged;
    _systemInfo.ExecuteDurationChanged -= OnExecuteDurationChanged;
    _systemInfo.CleanupDurationChanged -= OnCleanupDurationChanged;
    _systemInfo.TeardownDurationChanged -= OnTeardownDurationChanged;
    
    _activate.Pressed -= OnActivationChanged;
    _systemObserverNode = null;
    _systemInfo = null;
  }
}