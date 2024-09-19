using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Extensions;
using Godot;

namespace Entitas.Godot;

public class DebugSystems : Systems
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
  public SystemObserverNode Node => _node;

  public SystemInfo SystemInfo => _systemInfo;
  public double ExecuteDuration => _executeDuration;
  public double CleanupDuration => _cleanupDuration;
  
  public readonly List<SystemInfo> InitializeSystemInfos = new();
  public readonly List<SystemInfo> ExecuteSystemInfos = new();
  public readonly List<SystemInfo> CleanupSystemInfos = new();
  public readonly List<SystemInfo> TearDownSystemInfos = new();
  public readonly List<ISystem> AllSystems = new();

  private string _name;
  private SystemObserverNode _node;
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

  protected void Initialize(string name)
  {
    _name = name;
    _node = new SystemObserverNode();
    _systemInfo = new SystemInfo(this);
    _stopwatch = new Stopwatch();
    
    _node.Initialize(_systemInfo);
  }

  public override Systems Add(ISystem system)
  {
    AllSystems.Add(system);

    SystemInfo childSystemInfo;

    if (system is DebugSystems debugSystems)
    {
      childSystemInfo = debugSystems.SystemInfo;
      debugSystems.Node.Reparent(_node);
    }
    else
    {
      childSystemInfo = new SystemInfo(system);
      SystemObserverNode childNode = new();
      childNode.Initialize(childSystemInfo);
      childNode.Reparent(_node);
    }

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
    if (_node != null && _node.IsActive != _systemInfo.IsActive) _systemInfo.IsActive = _node.IsActive;
    if (_node is { IsActive: true })
      StepExecute();
  }

  public override void Cleanup()
  {
    if (_node != null && _node.IsActive != _systemInfo.IsActive) _systemInfo.IsActive = _node.IsActive;
    if (_node is { IsActive: true })
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
        var duration = _stopwatch.Elapsed.TotalMilliseconds;
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
}