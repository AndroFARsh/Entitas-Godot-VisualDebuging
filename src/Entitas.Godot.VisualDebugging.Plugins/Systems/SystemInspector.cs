using System;
using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;



public partial class SystemInspector : BaseInspector
{
  public class SystemPerformance : IDisposable
  {
    public SystemInterfaceFlags _interfaceFlags;
    public SystemInfo _systemInfo;
    
    public Label _average;
    public Label _min;
    public Label _max;

    public SystemPerformance(SystemInterfaceFlags interfaceFlags, SystemInfo systemInfo, Label average, Label min, Label max)
    {
      _interfaceFlags = interfaceFlags;
      _systemInfo = systemInfo;
      _average = average;
      _min = min;
      _max = max;
    }

    public void Dispose()
    {
      _systemInfo = null;
    
      _average?.GetParent()?.RemoveChild(_average);
      _min?.GetParent()?.RemoveChild(_min);
      _max?.GetParent()?.RemoveChild(_max);
      
      _average?.QueueFree();
      _min?.QueueFree();
      _max?.QueueFree();
    }
  }

  public class SystemRow : IDisposable
  {
    public CheckButton _checkButton;
    public Label _title;
    public Container _contaier;
    public SystemInfo _systemInfo;

    public SystemRow(CheckButton checkButton, Label title, Container contaier, SystemInfo systemInfo)
    {
      _checkButton = checkButton;
      _title = title;
      _contaier = contaier;
      _systemInfo = systemInfo;
      
      _checkButton.ButtonPressed = systemInfo.IsActive;
      _checkButton.Pressed += OnActiveButtonClicked;
    }
    
    public void OnActiveButtonClicked() => _systemInfo.IsActive = _checkButton.ButtonPressed;

    public void Dispose()
    {
      _checkButton.GetParent().RemoveChild(_checkButton);
      _checkButton.Pressed -= OnActiveButtonClicked;
      _checkButton.QueueFree();
      
      _title.GetParent().RemoveChild(_title);
      _title.QueueFree();
      
      _contaier.GetParent().RemoveChild(_contaier);
      _contaier.QueueFree();
      
      _systemInfo = null;
    }
  }

  public class SystemsData
  {
    public bool _prevVisibility = true;
    
    public CheckButton _checkButton;
    public Container _content;
    public GridContainer _gridContainer;
    
    public List<SystemRow> _rows = new ();

    public SystemsData(CheckButton checkButton, Container content, GridContainer gridContainer)
    {
      _checkButton = checkButton;
      _content = content;
      _gridContainer = gridContainer;

      _checkButton.ButtonPressed = _prevVisibility;
      _content.Visible = _prevVisibility;
      _checkButton.Pressed += OnActiveButtonClicked;
    }

    private void OnActiveButtonClicked()
    {
      _prevVisibility = _checkButton.ButtonPressed;
      _content.Visible = _prevVisibility;
    } 
  }

  private const string DurationFormat = "00.0000";
  private const string PlayIcon = "\u23f5";
  private const string PauseIcon = "\u23f8";
  private const string StepIcon = "\u23ef";
  
  private SystemInfo _systemInfo;
  private DebugSystems _systems;
  
  // Details
  private CheckButton _detailsCheckButton;
  private MarginContainer _detailsContent;
  private Label _systemTitle;
  
  private Label _initializeSystemCount;
  private Label _executeSystemCount;
  private Label _cleanupSystemCount;
  private Label _tearDownSystemCount;
  private Label _totalSystemCount;
  
  private int _prevInitializeSystemCount;
  private int _prevExecuteSystemCount;
  private int _prevCleanupSystemCount;
  private int _prevTearDownSystemCount;
  private int _prevTotalSystemCount;
  
  private bool _prevDetailsVisibility;
  
  // Performance
  private CheckButton _performanceCheckButton;
  private MarginContainer _performanceContent;
  private Label _averageExecuteSystemDuration;
  private Label _minExecuteSystemDuration;
  private Label _maxExecuteSystemDuration;
  
  private Label _averageCleanupSystemDuration;
  private Label _minCleanupSystemDuration;
  private Label _maxCleanupSystemDuration;
  
  private Button _pauseButton;
  private Button _stepButton;
  private bool _prevPerformanceVisibility = true;
  
  // Systems
  private CheckButton _systemsCheckButton;
  private MarginContainer _systemsContent;
  private OptionButton _resetOptionMode;
  private Button _resetAverageNowButton;
  
  private bool _prevSystemsVisibility = true;
  
  private TextEdit _systemFilterTextEdit;
  private Button _systemFilterClearButton;

  private Dictionary<SystemInterfaceFlags, SystemsData> _systemDatas = new();
  private List<SystemPerformance> _systemPerformances = new();
  
  public void Initialize(DebugSystems systems)
  {
    if (_systems == systems) return;
    _systems = systems;
    _systemInfo = systems.SystemInfo;

    InitializeTree();

    // Details
    _detailsCheckButton.Pressed += OnDetailsVisibilityChanged;
    _detailsCheckButton.ButtonPressed = _prevDetailsVisibility;
    _detailsContent.Visible = _prevDetailsVisibility;
    _systemTitle.Text = _systemInfo.SystemName;
    _prevInitializeSystemCount = -1;
    _prevExecuteSystemCount = -1;
    _prevCleanupSystemCount = -1;
    _prevTearDownSystemCount = -1;
    _prevTotalSystemCount = -1;
    
    // Performance
    _performanceCheckButton.Pressed += OnPerformanceVisibilityChanged;
    _performanceCheckButton.ButtonPressed = _prevPerformanceVisibility;
    _performanceContent.Visible = _prevPerformanceVisibility;
    
    _pauseButton.Pressed += OnPauseButtonClicked;
    _pauseButton.Text = _systemInfo.IsActive ? PauseIcon : PlayIcon;
    
    _stepButton.Pressed += OnStepButtonClicked;
    _stepButton.Disabled = _systemInfo.IsActive;
    
    // Systems
    _systemsCheckButton.Pressed += OnSystemVisibilityChanged;
    _systemsCheckButton.ButtonPressed = _prevSystemsVisibility;
    _systemsContent.Visible = _prevSystemsVisibility;

    _resetOptionMode.Selected = DebugSystems.AvgResetInterval.IndexOf();
    _resetOptionMode.ItemSelected += OnResetItemOptionSelected;
    _resetAverageNowButton.Pressed += OnResetNowButtonPressed;

    _systemFilterTextEdit.TextChanged += OnSystemFilterTextChanged;
    _systemFilterClearButton.Pressed += OnSystemFilterClearClicked;

    FillSystem(_systems);
  }

  private void FillSystem(DebugSystems systems)
  {
    FillSystem("", SystemInterfaceFlags.InitializeSystem, systems);
    FillSystem("", SystemInterfaceFlags.ExecuteSystem, systems);
    FillSystem("", SystemInterfaceFlags.CleanupSystem, systems);
    FillSystem("", SystemInterfaceFlags.TearDownSystem, systems);
  }

  private void FillSystem(string offset, SystemInterfaceFlags interfaceFlags, DebugSystems systems)
  {
    foreach (SystemInfo systemInfo in systems.GetSystemInfo(interfaceFlags))
    {
      SystemsData data = _systemDatas[interfaceFlags];
      
      CheckButton checkButton = new();
      data._gridContainer.AddChild(checkButton);
      
      Label label = new();
      label.Text = offset + systemInfo.SystemName;
      data._gridContainer.AddChild(label);
      
      HBoxContainer container = new();
      data._gridContainer.AddChild(container);

      SystemRow row = new(checkButton, label, container, systemInfo);
      data._rows.Add(row);
      
      if (interfaceFlags is SystemInterfaceFlags.ExecuteSystem or SystemInterfaceFlags.CleanupSystem)
        _systemPerformances.Add(new SystemPerformance(
          interfaceFlags, 
          systemInfo,
          CreateDurationLabel(row._contaier, "\u2300", "ms"),
          CreateDurationLabel(row._contaier, "\u2193", "ms"),
          CreateDurationLabel(row._contaier, "\u2191", "ms")
        ));
      else
        _systemPerformances.Add(new SystemPerformance(
          interfaceFlags, 
          systemInfo,
          CreateDurationLabel(row._contaier, "", "ms"),
          null,
          null
        ));
      
      if (systemInfo.System is DebugSystems childSystem)
        FillSystem(offset + "    ", interfaceFlags, childSystem);
    }
  }

  public override void _Process(double delta)
  {
    UpdateDetails();
    UpdatePerformance();
    UpdateSystems();
  }
  
  private void UpdateSystems()
  {
    if (!_prevSystemsVisibility) return;

    foreach (SystemPerformance performance in _systemPerformances)
    {
      switch (performance._interfaceFlags)
      {
        case SystemInterfaceFlags.InitializeSystem:
          performance._average.Text = performance._systemInfo.InitializationDuration.ToString(DurationFormat);
          break;
        case SystemInterfaceFlags.ExecuteSystem:
          performance._average.Text = performance._systemInfo.AverageExecutionDuration.ToString(DurationFormat);
          performance._min.Text = performance._systemInfo.MinExecutionDuration.ToString(DurationFormat);
          performance._max.Text = performance._systemInfo.MaxExecutionDuration.ToString(DurationFormat);
          break;
        case SystemInterfaceFlags.CleanupSystem:
          performance._average.Text = performance._systemInfo.AverageCleanupDuration.ToString(DurationFormat);
          performance._min.Text = performance._systemInfo.MinCleanupDuration.ToString(DurationFormat);
          performance._max.Text = performance._systemInfo.MaxCleanupDuration.ToString(DurationFormat);
          break;
        case SystemInterfaceFlags.TearDownSystem:
          performance._average.Text = performance._systemInfo.TeardownDuration.ToString(DurationFormat);
          break;
      }
    }
  }
  
  private void UpdatePerformance()
  {
    if (!_prevPerformanceVisibility) return;
    
    _averageExecuteSystemDuration.Text = _systemInfo.AverageExecutionDuration.ToString(DurationFormat);
    _minExecuteSystemDuration.Text = _systemInfo.MinExecutionDuration.ToString(DurationFormat);
    _maxExecuteSystemDuration.Text = _systemInfo.MaxExecutionDuration.ToString(DurationFormat);
    
    _averageCleanupSystemDuration.Text = _systemInfo.AverageCleanupDuration.ToString(DurationFormat);
    _minCleanupSystemDuration.Text = _systemInfo.MinCleanupDuration.ToString(DurationFormat);
    _maxCleanupSystemDuration.Text = _systemInfo.MaxCleanupDuration.ToString(DurationFormat);
  }

  private void UpdateDetails()
  {
    // init
    if (_prevInitializeSystemCount != _systems.TotalInitializeSystemsCount)
    {
      _prevInitializeSystemCount = _systems.TotalInitializeSystemsCount;
      _initializeSystemCount.Text = _prevInitializeSystemCount.ToString();  
    }

    // exec
    if (_prevExecuteSystemCount != _systems.TotalExecuteSystemsCount)
    {
      _prevExecuteSystemCount = _systems.TotalExecuteSystemsCount;
      _executeSystemCount.Text = _prevExecuteSystemCount.ToString();  
    }
    
    // cleanup
    if (_prevCleanupSystemCount != _systems.TotalCleanupSystemsCount)
    {
      _prevCleanupSystemCount = _systems.TotalCleanupSystemsCount;
      _cleanupSystemCount.Text = _prevCleanupSystemCount.ToString();  
    }
    
    // tearDown
    if (_prevTearDownSystemCount != _systems.TotalTearDownSystemsCount)
    {
      _prevTearDownSystemCount = _systems.TotalTearDownSystemsCount;
      _tearDownSystemCount.Text = _prevTearDownSystemCount.ToString();  
    }
    
    // total
    if (_prevTotalSystemCount != _systems.TotalSystemsCount)
    {
      _prevTotalSystemCount = _systems.TotalSystemsCount;
      _totalSystemCount.Text = _prevTotalSystemCount.ToString();  
    }
  }

  public override void CleanUp()
  {
    if (_systems == null) return;

    _detailsCheckButton.Pressed -= OnDetailsVisibilityChanged;
    
    _performanceCheckButton.Pressed -= OnPerformanceVisibilityChanged;
    _pauseButton.Pressed -= OnPauseButtonClicked;
    _stepButton.Pressed -= OnStepButtonClicked;
    
    // Systems
    _systemsCheckButton.Pressed -= OnSystemVisibilityChanged;
    _resetOptionMode.ItemSelected -= OnResetItemOptionSelected;
    _resetAverageNowButton.Pressed -= OnResetNowButtonPressed;

    _systemFilterTextEdit.TextChanged -= OnSystemFilterTextChanged;
    _systemFilterClearButton.Pressed -= OnSystemFilterClearClicked;
    
    foreach (SystemsData data in _systemDatas.Values)
    {
      foreach (SystemRow row in data._rows)
        row.Dispose();
      
      data._rows.Clear();
    }
    
    foreach (SystemPerformance performance in _systemPerformances)
    {
      performance.Dispose();
    }
    _systemPerformances.Clear();
    
    _systemInfo = null;
    _systems = null;
  }

  private void InitializeTree()
  {
    if (GetChildCount() > 0) return;
    
    AddThemeConstantOverride("margin_top", Consts.Margin);
    AddThemeConstantOverride("margin_left", Consts.Margin);
    AddThemeConstantOverride("margin_bottom", Consts.Margin);
    AddThemeConstantOverride("margin_right", Consts.Margin);
    AnchorsPreset = (int) LayoutPreset.FullRect;
    GrowHorizontal = GrowDirection.Both;
    GrowVertical = GrowDirection.Both;
    SizeFlagsHorizontal = SizeFlags.ExpandFill;

    VBoxContainer container = new();
    container.AddThemeConstantOverride("separation", Consts.Margin);
    container.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    AddChild(container);

    InitializeDetailsTree(container);
    InitializePerformanceTree(container);
    InitializeSystemsTree(container);
  }

  private void InitializeDetailsTree(Control parent)
  {
    PanelContainer panelContainer = new();
    panelContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    parent.AddChild(panelContainer);

    VBoxContainer vBoxContainer = new();
    panelContainer.AddChild(vBoxContainer);

    _detailsCheckButton = new CheckButton();
    _detailsCheckButton.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    _detailsCheckButton.Text = "Details";
    _detailsCheckButton.Alignment = HorizontalAlignment.Center;
    vBoxContainer.AddChild(_detailsCheckButton);

    _detailsContent = new MarginContainer();
    _detailsContent.AddThemeConstantOverride("margin_top", Consts.Margin);
    _detailsContent.AddThemeConstantOverride("margin_left", Consts.Margin);
    _detailsContent.AddThemeConstantOverride("margin_bottom", Consts.Margin);
    _detailsContent.AddThemeConstantOverride("margin_right", Consts.Margin);
    vBoxContainer.AddChild(_detailsContent);
    
    GridContainer gridContainer = new();
    gridContainer.AddThemeConstantOverride("h_separation", Consts.Margin);
    gridContainer.AddThemeConstantOverride("v_separation", Consts.Margin);
    gridContainer.Columns = 2;
    _detailsContent.AddChild(gridContainer);

    _systemTitle = new Label();
    _systemTitle.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    gridContainer.AddChild(_systemTitle);

    Control stub = new();
    gridContainer.AddChild(stub);
    
    _initializeSystemCount = CreateSystemCount(gridContainer, "Initialize System:");
    _executeSystemCount = CreateSystemCount(gridContainer, "Execute System:");
    _cleanupSystemCount = CreateSystemCount(gridContainer, "CleanUp System:");
    _tearDownSystemCount = CreateSystemCount(gridContainer, "TearDown System:");
    _totalSystemCount = CreateSystemCount(gridContainer, "Total System:");
  }

  private static Label CreateSystemCount(GridContainer gridContainer, string text)
  {
    Label title = new();
    title.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    title.Text = text;
    gridContainer.AddChild(title);
    
    Label count = new();
    count.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    gridContainer.AddChild(count);

    return count;
  }
  
  private void OnDetailsVisibilityChanged()
  {
    _prevDetailsVisibility = _detailsCheckButton.ButtonPressed;
    _detailsContent.Visible = _prevDetailsVisibility;
  }
  
  private void InitializePerformanceTree(Control parent)
  {
    PanelContainer panelContainer = new();
    panelContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    parent.AddChild(panelContainer);

    VBoxContainer vBoxContainer = new();
    panelContainer.AddChild(vBoxContainer);

    _performanceCheckButton = new CheckButton();
    _performanceCheckButton.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    _performanceCheckButton.Text = "Performance";
    _performanceCheckButton.Alignment = HorizontalAlignment.Center;
    vBoxContainer.AddChild(_performanceCheckButton);

    _performanceContent = new MarginContainer();
    _performanceContent.AddThemeConstantOverride("margin_top", Consts.Margin);
    _performanceContent.AddThemeConstantOverride("margin_left", Consts.Margin);
    _performanceContent.AddThemeConstantOverride("margin_bottom", Consts.Margin);
    _performanceContent.AddThemeConstantOverride("margin_right", Consts.Margin);
    vBoxContainer.AddChild(_performanceContent);
    
    VBoxContainer vBox = new();
    vBox.AddThemeConstantOverride("separation", Consts.Margin);
    _performanceContent.AddChild(vBox);
    
    GridContainer gridContainer = new();
    gridContainer.AddThemeConstantOverride("h_separation", Consts.Margin);
    gridContainer.AddThemeConstantOverride("v_separation", Consts.Margin);
    gridContainer.Columns = 4;
    vBox.AddChild(gridContainer);

    CreateDurationTitle(gridContainer, "Execute Duration:");
    _averageExecuteSystemDuration = CreateDurationLabel(gridContainer, "\u2300", "ms");
    _minExecuteSystemDuration = CreateDurationLabel(gridContainer, "\u2193", "ms");
    _maxExecuteSystemDuration = CreateDurationLabel(gridContainer, "\u2191", "ms");
    
    CreateDurationTitle(gridContainer, "CleanUp Duration:");
    _averageCleanupSystemDuration = CreateDurationLabel(gridContainer, "\u2300", "ms");
    _minCleanupSystemDuration = CreateDurationLabel(gridContainer, "\u2193", "ms");
    _maxCleanupSystemDuration = CreateDurationLabel(gridContainer, "\u2191", "ms");

    HBoxContainer hBox = new();
    hBox.AddThemeConstantOverride("separation", Consts.Margin);
    vBox.AddChild(hBox);

    _pauseButton = new Button();
    _pauseButton.CustomMinimumSize = new Vector2(40, 40);
    _pauseButton.Text = PauseIcon;
    hBox.AddChild(_pauseButton);
    
    _stepButton = new Button();
    _stepButton.CustomMinimumSize = new Vector2(40, 40);
    _stepButton.Text = StepIcon;
    hBox.AddChild(_stepButton);
  }

  private static Label CreateDurationLabel(Container container, string suffix, string prefix)
  {
    HBoxContainer hBox = new();
    container.AddChild(hBox);
    
    Label suffixLabel = new();
    suffixLabel.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    suffixLabel.Text = suffix;
    hBox.AddChild(suffixLabel);
    
    Label contentLabel = new();
    contentLabel.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    hBox.AddChild(contentLabel);
    
    Label prefixLabel = new();
    prefixLabel.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    prefixLabel.Text = prefix;
    hBox.AddChild(prefixLabel);

    return contentLabel;
  }

  private static void CreateDurationTitle(Container container, string text)
  {
    Label title = new();
    title.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    title.Text = text;
    container.AddChild(title);
  }
  
  private void OnPerformanceVisibilityChanged()
  {
    _prevPerformanceVisibility = _performanceCheckButton.ButtonPressed;
    _performanceContent.Visible = _prevPerformanceVisibility;
  }

  private void OnStepButtonClicked()
  {
    _systems.StepExecute();
    _systems.StepCleanup();
  }

  private void OnPauseButtonClicked()
  {
    _systemInfo.IsActive = !_systemInfo.IsActive;
    _stepButton.Disabled = _systemInfo.IsActive;
    _pauseButton.Text = _systemInfo.IsActive ? PauseIcon : PlayIcon;
  }
  
  private void InitializeSystemsTree(Control parent)
  {
    PanelContainer panelContainer = new();
    panelContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    parent.AddChild(panelContainer);

    VBoxContainer vBoxContainer = new();
    panelContainer.AddChild(vBoxContainer);

    _systemsCheckButton = new CheckButton();
    _systemsCheckButton.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    _systemsCheckButton.Text = "Systems";
    _systemsCheckButton.Alignment = HorizontalAlignment.Center;
    vBoxContainer.AddChild(_systemsCheckButton);

    _systemsContent = new MarginContainer();
    _systemsContent.AddThemeConstantOverride("margin_top", Consts.Margin);
    _systemsContent.AddThemeConstantOverride("margin_left", Consts.Margin);
    _systemsContent.AddThemeConstantOverride("margin_bottom", Consts.Margin);
    _systemsContent.AddThemeConstantOverride("margin_right", Consts.Margin);
    vBoxContainer.AddChild(_systemsContent);
    
    VBoxContainer vBox = new();
    vBox.AddThemeConstantOverride("separation", Consts.Margin);
    _systemsContent.AddChild(vBox);

    InitializeResetAverageTree(vBox);
    InitializeFilterSystemTree(vBox);
    
    _systemDatas.Add(SystemInterfaceFlags.InitializeSystem, InitializeSystemTree(vBox, "Initialize Systems"));
    _systemDatas.Add(SystemInterfaceFlags.ExecuteSystem, InitializeSystemTree(vBox, "Execute Systems"));
    _systemDatas.Add(SystemInterfaceFlags.CleanupSystem, InitializeSystemTree(vBox, "Cleanup Systems"));
    _systemDatas.Add(SystemInterfaceFlags.TearDownSystem, InitializeSystemTree(vBox, "TearDown Systems"));
  }

  private void InitializeResetAverageTree(Container container)
  {
    HBoxContainer hBox = new();
    hBox.AddThemeConstantOverride("separation", Consts.Margin);
    hBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    container.AddChild(hBox);
    
    Label title = new();
    title.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    title.Text = "Reset Average Duration \u2300:";
    hBox.AddChild(title);

    _resetOptionMode = new OptionButton();
    _resetOptionMode.CustomMinimumSize = new Vector2(0, 50);
    _resetOptionMode.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    hBox.AddChild(_resetOptionMode);
    
    foreach (string resetInterval in Enum.GetNames<AvgResetInterval>())
      _resetOptionMode.AddItem(resetInterval);
    
    _resetAverageNowButton = new Button();
    _resetAverageNowButton.CustomMinimumSize = new Vector2(40, 40);
    _resetAverageNowButton.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    _resetAverageNowButton.Text = "Reset ⌀ Now";
    hBox.AddChild(_resetAverageNowButton);
  }
  
  private void InitializeFilterSystemTree(Container container)
  {
    HBoxContainer hBox = new();
    hBox.AddThemeConstantOverride("separation", Consts.Margin);
    hBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    container.AddChild(hBox);
    
    Label title = new();
    title.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    title.Text = "\ud83d\udd0d";
    hBox.AddChild(title);

    _systemFilterTextEdit = new TextEdit();
    _systemFilterTextEdit.CustomMinimumSize = new Vector2(0, 50);
    _systemFilterTextEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    hBox.AddChild(_systemFilterTextEdit);
    
    _systemFilterClearButton = new Button();
    _systemFilterClearButton.CustomMinimumSize = new Vector2(40, 40);
    _systemFilterClearButton.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    _systemFilterClearButton.Text = "\u26cc";
    hBox.AddChild(_systemFilterClearButton);
  }
  
  private SystemsData InitializeSystemTree(Container parent, string title)
  {
    VBoxContainer vBoxContainer = new();
    parent.AddChild(vBoxContainer);

    CheckButton checkButton = new();
    checkButton.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    checkButton.Text = title;
    checkButton.Alignment = HorizontalAlignment.Center;
    vBoxContainer.AddChild(checkButton);

    MarginContainer content = new();
    content.AddThemeConstantOverride("margin_top", Consts.Margin);
    content.AddThemeConstantOverride("margin_left", Consts.Margin);
    content.AddThemeConstantOverride("margin_bottom", Consts.Margin);
    content.AddThemeConstantOverride("margin_right", Consts.Margin);
    vBoxContainer.AddChild(content);
    
    GridContainer gridContainer = new();
    gridContainer.Columns = 3;
    gridContainer.AddThemeConstantOverride("h_separation", Consts.Margin);
    gridContainer.AddThemeConstantOverride("v_separation", Consts.Margin);
    content.AddChild(gridContainer);
    
    return new SystemsData(checkButton, content, gridContainer);
  }
  
  private void OnSystemVisibilityChanged()
  {
    _prevSystemsVisibility = _systemsCheckButton.ButtonPressed;
    _systemsContent.Visible = _prevSystemsVisibility;
  }
  
  private void OnResetItemOptionSelected(long index) =>
    DebugSystems.AvgResetInterval = EnumExtensions.FromIndex<AvgResetInterval>((int)index);

  private void OnResetNowButtonPressed() => _systems.ResetDurations();
  
  private void OnSystemFilterClearClicked()
  {
    _systemFilterTextEdit.Text = "";
    OnSystemFilterTextChanged();
  }

  private void OnSystemFilterTextChanged()
  {
    string searchText = _systemFilterTextEdit.Text.ToLower();

    foreach ((SystemInterfaceFlags _, SystemsData data) in _systemDatas)
    foreach (SystemRow row in data._rows)
    {
      bool visibility = row._systemInfo.SystemName.ToLower().Contains(searchText);
      row._checkButton.Visible = visibility;
      row._title.Visible = visibility;
      row._contaier.Visible = visibility;
    } 
  }
}