using Godot;

namespace Entitas.Godot;

public partial class SystemObserverNode : Node
{
  [Export]
  public bool IsActive
  {
    get => _systemInfo.IsActive;
    set => _systemInfo.IsActive = value;
  }

  private SystemInfo _systemInfo;
  
  public SystemInfo SystemInfo => _systemInfo;
  
  public void Initialize(SystemInfo systemInfo)
  {
    Name = systemInfo.SystemName;
    _systemInfo = systemInfo;
    EntitasRoot.Systems.AddChild(this);
  }
}