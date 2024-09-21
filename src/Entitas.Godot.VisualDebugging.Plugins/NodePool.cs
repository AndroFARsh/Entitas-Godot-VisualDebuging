using System;
using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

public class NodePool : IDisposable
{
  private readonly Dictionary<Type, Stack<Node>> _pools = new();

  public bool Request<T>(Type type, out T node) where T : Node
  {
    if (!_pools.TryGetValue(type, out Stack<Node> stack))
      stack = new Stack<Node>();

    node = stack.Count > 0 ? (T)stack.Pop() : default;
    return node != null;
  }

  public void Retain(Type type, Node node)
  {
    if (!_pools.TryGetValue(type, out Stack<Node> stack))
      stack = new Stack<Node>();
    
    stack.Push(node);
  }

  public void Dispose()
  {
    foreach (Stack<Node> pool in _pools.Values)
    foreach (Node obj in pool)
    {
      obj.QueueFree();  
    }
    _pools.Clear();
  }
}