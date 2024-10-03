using Extensions;

namespace Entitas.Godot;

public class SystemInfo
{
  public bool IsInitializeSystems => 
    (_interfaceFlags & SystemInterfaceFlags.InitializeSystem) == SystemInterfaceFlags.InitializeSystem;
  
  public bool IsExecuteSystems =>
    (_interfaceFlags & SystemInterfaceFlags.ExecuteSystem) == SystemInterfaceFlags.ExecuteSystem;

  public bool IsCleanupSystems =>
    (_interfaceFlags & SystemInterfaceFlags.CleanupSystem) == SystemInterfaceFlags.CleanupSystem;

  public bool IsTearDownSystems =>
    (_interfaceFlags & SystemInterfaceFlags.TearDownSystem) == SystemInterfaceFlags.TearDownSystem;

  public bool IsDebugSystems =>
    (_interfaceFlags & SystemInterfaceFlags.DebugSystem) == SystemInterfaceFlags.DebugSystem;

  
  public bool IsReactiveSystems =>
    (_interfaceFlags & SystemInterfaceFlags.ReactiveSystem) == SystemInterfaceFlags.ReactiveSystem;

  public double InitializationDuration { get; set; }

  public double TeardownDuration { get; set; }

  public double AccumulatedExecutionDuration => _accumulatedExecutionDuration;
  public double MinExecutionDuration => _minExecutionDuration;
  public double MaxExecutionDuration => _maxExecutionDuration;

  public double AverageExecutionDuration =>
    _executionDurationsCount != 0 ? _accumulatedExecutionDuration / _executionDurationsCount : 0;

  public double AccumulatedCleanupDuration => _accumulatedCleanupDuration;
  public double MinCleanupDuration => _minCleanupDuration;
  public double MaxCleanupDuration => _maxCleanupDuration;

  public double AverageCleanupDuration =>
    _cleanupDurationsCount != 0 ? _accumulatedCleanupDuration / _cleanupDurationsCount : 0;

  public bool AreAllParentsActive =>
    ParentSystemInfo == null || (ParentSystemInfo.IsActive && ParentSystemInfo.AreAllParentsActive);

  public SystemInfo ParentSystemInfo;
  public bool IsActive;

  public readonly ISystem System;
  public readonly string SystemName;
  
  private readonly SystemInterfaceFlags _interfaceFlags;

  private double _accumulatedExecutionDuration;
  
  private double _minExecutionDuration;
  private double _maxExecutionDuration;
  private int _executionDurationsCount;

  private double _accumulatedCleanupDuration;
  private double _minCleanupDuration;
  private double _maxCleanupDuration;
  private int _cleanupDurationsCount;
  
  public SystemInfo(ISystem system)
  {
    System = system;
    _interfaceFlags = GetInterfaceFlags(system);

    SystemName = system is DebugFeature debugSystem && debugSystem.Name.Length > 0
      ? debugSystem.Name
      : system.GetType().TypeName() ?? nameof(system);
    
    IsActive = (system as IDebugSystem)?.IsActive ?? true;
  }

  public void AddExecutionDuration(double executionDuration)
  {
    if (executionDuration < _minExecutionDuration || _minExecutionDuration == 0)
      _minExecutionDuration = executionDuration;

    if (executionDuration > _maxExecutionDuration)
      _maxExecutionDuration = executionDuration;

    _accumulatedExecutionDuration += executionDuration;
    _executionDurationsCount += 1;
  }

  public void AddCleanupDuration(double cleanupDuration)
  {
    if (cleanupDuration < _minCleanupDuration || _minCleanupDuration == 0)
      _minCleanupDuration = cleanupDuration;

    if (cleanupDuration > _maxCleanupDuration)
      _maxCleanupDuration = cleanupDuration;

    _accumulatedCleanupDuration += cleanupDuration;
    _cleanupDurationsCount += 1;
  }

  public void ResetDurations()
  {
    _accumulatedExecutionDuration = 0;
    _executionDurationsCount = 0;

    _accumulatedCleanupDuration = 0;
    _cleanupDurationsCount = 0;
  }

  static SystemInterfaceFlags GetInterfaceFlags(ISystem system)
  {
    SystemInterfaceFlags flags = SystemInterfaceFlags.None;
    if (system is IInitializeSystem) flags |= SystemInterfaceFlags.InitializeSystem;
    
    if (system is IReactiveSystem) flags |= SystemInterfaceFlags.ReactiveSystem;
    else if (system is IExecuteSystem) flags |= SystemInterfaceFlags.ExecuteSystem;
    
    if (system is ICleanupSystem) flags |= SystemInterfaceFlags.CleanupSystem;
    if (system is ITearDownSystem) flags |= SystemInterfaceFlags.TearDownSystem;

    if (system is IDebugSystem) flags |= SystemInterfaceFlags.DebugSystem;
    
    return flags;
  }
}