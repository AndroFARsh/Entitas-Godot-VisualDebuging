#if DEBUG
namespace Entitas.Godot;

// debug 
public interface IDebugSystem : ISystem
{
  bool IsActive => false;
}

#endif