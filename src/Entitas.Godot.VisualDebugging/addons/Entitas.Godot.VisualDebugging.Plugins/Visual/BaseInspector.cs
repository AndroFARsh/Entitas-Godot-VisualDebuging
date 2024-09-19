using Godot;

namespace Entitas.Godot;

public abstract partial class BaseInspector : Control 
{
  public abstract void Initialize(Node node);
  
  public abstract void CleanUp();
}