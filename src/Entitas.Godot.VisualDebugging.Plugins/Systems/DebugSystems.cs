using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Extensions;
using Godot;

namespace Entitas.Godot;

public class DebugSystems : Systems, IDisposable
{
  public static AvgResetInterval AvgResetInterval = AvgResetInterval.Never;

  public int TotalInitializeSystemsCount => _initializeSystems.Sum(system =>
    system is DebugSystems debugSystems ? debugSystems.TotalInitializeSystemsCount : 1);

  public int TotalExecuteSystemsCount => _executeSystems.Sum(system =>
    system is DebugSystems debugSystems ? debugSystems.TotalExecuteSystemsCount : 1);

  public int TotalCleanupSystemsCount => _cleanupSystems.Sum(system =>
    system is DebugSystems debugSystems ? debugSystems.TotalCleanupSystemsCount : 1);

  public int TotalTearDownSystemsCount => _tearDownSystems.Sum(system =>
    system is DebugSystems debugSystems ? debugSystems.TotalTearDownSystemsCount : 1);

  public int TotalSystemsCount =>
    AllSystems.Sum(system => system is DebugSystems debugSystems ? debugSystems.TotalSystemsCount : 1);

  public int InitializeSystemsCount => _initializeSystems.Count;
  public int ExecuteSystemsCount => _executeSystems.Count;
  public int CleanupSystemsCount => _cleanupSystems.Count;
  public int TearDownSystemsCount => _tearDownSystems.Count;

  public string Name => _name;

  public SystemInfo SystemInfo => _systemInfo;
  public double ExecuteDuration => _executeDuration;
  public double CleanupDuration => _cleanupDuration;
  
  public readonly List<SystemInfo> InitializeSystemInfos = new();
  public readonly List<SystemInfo> ExecuteSystemInfos = new();
  public readonly List<SystemInfo> CleanupSystemInfos = new();
  public readonly List<SystemInfo> TearDownSystemInfos = new();
  public readonly List<ISystem> AllSystems = new();
  
  private readonly List<SystemInfo> _systems = new();
  public List<SystemInfo> GetSystemInfo(SystemInterfaceFlags flags)
  {
    _systems.Clear();
    
    if ((flags & SystemInterfaceFlags.InitializeSystem) > 0) _systems.AddRange(InitializeSystemInfos);
    if ((flags & SystemInterfaceFlags.ExecuteSystem) > 0) _systems.AddRange(ExecuteSystemInfos);
    if ((flags & SystemInterfaceFlags.CleanupSystem) > 0) _systems.AddRange(CleanupSystemInfos);
    if ((flags & SystemInterfaceFlags.TearDownSystem) > 0) _systems.AddRange(TearDownSystemInfos);

    return _systems;
  }

  private string _name;
  private SystemInfo _systemInfo;
  private Stopwatch _stopwatch;
  private double _executeDuration;
  private double _cleanupDuration;

  public DebugSystems(string name)
  {
    Initialize(name);
  }

  protected DebugSystems()
  {
    Initialize(GetType().TypeName());
  }
  
  ~DebugSystems() 
  {
    GD.PrintErr("~DebugSystems");
    Dispose();
  }

  protected void Initialize(string name)
  {
    _name = name;
    _systemInfo = new SystemInfo(this);
    _stopwatch = new Stopwatch();
    
    EntitasRoot.RegisterSystem(this);
  }

  public override Systems Add(ISystem system)
  {
    AllSystems.Add(system);

    SystemInfo childSystemInfo = system is DebugSystems debugSystems
      ? debugSystems.SystemInfo
      : new SystemInfo(system);
    
    if (system is DebugSystems childDebugSystems)
      EntitasRoot.UnregisterTopSystem(childDebugSystems);
    
    childSystemInfo.ParentSystemInfo = _systemInfo;

    if (childSystemInfo.IsInitializeSystems) InitializeSystemInfos.Add(childSystemInfo);
    if (childSystemInfo.IsExecuteSystems || childSystemInfo.IsReactiveSystems) ExecuteSystemInfos.Add(childSystemInfo);
    if (childSystemInfo.IsCleanupSystems) CleanupSystemInfos.Add(childSystemInfo);
    if (childSystemInfo.IsTearDownSystems) TearDownSystemInfos.Add(childSystemInfo);

    return base.Add(system);
  }

  public void ResetDurations()
  {
    foreach (var systemInfo in ExecuteSystemInfos)
      systemInfo.ResetDurations();

    foreach (var system in AllSystems)
      if (system is DebugSystems debugSystems)
        debugSystems.ResetDurations();
  }

  public override void Initialize()
  {
    if (!SystemInfo.IsActive) return; 
    
    for (var i = 0; i < _initializeSystems.Count; i++)
    {
      SystemInfo systemInfo = InitializeSystemInfos[i];
      if (systemInfo.IsActive)
      {
        _stopwatch.Reset();
        _stopwatch.Start();
        _initializeSystems[i].Initialize();
        _stopwatch.Stop();
        systemInfo.InitializationDuration = _stopwatch.Elapsed.TotalMilliseconds;
      }
    }
  }

  public override void Execute()
  {
    if (SystemInfo.IsActive) 
      StepExecute();
  } 

  public override void Cleanup()
  {
    if (!SystemInfo.IsActive)
      StepCleanup();
  }

  public void StepExecute()
  {
    double executeDuration = 0d;
    if (Engine.GetFramesDrawn() % (int)AvgResetInterval == 0)
      ResetDurations();

    for (int i = 0; i < _executeSystems.Count; i++)
    {
      SystemInfo systemInfo = ExecuteSystemInfos[i];
      if (systemInfo.IsActive)
      {
        _stopwatch.Reset();
        _stopwatch.Start();
        _executeSystems[i].Execute();
        _stopwatch.Stop();
        double duration = _stopwatch.Elapsed.TotalMilliseconds;
        executeDuration += duration;
        systemInfo.AddExecutionDuration(duration);
      }
    }

    _executeDuration = executeDuration;
  }

  public void StepCleanup()
  {
    double cleanupDuration = 0d;
    for (int i = 0; i < _cleanupSystems.Count; i++)
    {
      SystemInfo systemInfo = CleanupSystemInfos[i];
      if (systemInfo.IsActive)
      {
        _stopwatch.Reset();
        _stopwatch.Start();
        _cleanupSystems[i].Cleanup();
        _stopwatch.Stop();
        double duration = _stopwatch.Elapsed.TotalMilliseconds;
        cleanupDuration += duration;
        systemInfo.AddCleanupDuration(duration);
      }
    }

    _cleanupDuration = cleanupDuration;
  }

  public override void TearDown()
  {
    if (!SystemInfo.IsActive) return;
    
    for (int i = 0; i < _tearDownSystems.Count; i++)
    {
      SystemInfo systemInfo = TearDownSystemInfos[i];
      if (systemInfo.IsActive)
      {
        _stopwatch.Reset();
        _stopwatch.Start();
        _tearDownSystems[i].TearDown();
        _stopwatch.Stop();
        systemInfo.TeardownDuration = _stopwatch.Elapsed.TotalMilliseconds;
      }
    }
  }

  public void Dispose()
  {
    foreach (ISystem system in AllSystems)
      if (system is DebugSystems childDebugSystem)
        childDebugSystem.Dispose();

    EntitasRoot.UnregisterSystem(this);
    
    DeactivateReactiveSystems();
    ClearReactiveSystems();
    
    _initializeSystems.Clear();
    _executeSystems.Clear();
    _cleanupSystems.Clear();
    _tearDownSystems.Clear();
  }
}