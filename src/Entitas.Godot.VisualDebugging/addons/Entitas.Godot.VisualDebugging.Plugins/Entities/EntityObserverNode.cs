using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Entitas.Godot;

public enum ComponentActionType
{
  Added,
  Replaced,
  Removed
}

public partial class EntityObserverNode : Node
{
  private readonly Dictionary<Type, ComponentInfo> _componentInfos = new();
  private readonly List<ComponentInfo> _sortedComponentInfos = new();
  
  private IContext _context;
  private IEntity _entity;
  
  public IContext Context => _context;
  public IEntity Entity => _entity;
  public List<ComponentInfo> Components => _sortedComponentInfos;

  public event Action<ComponentActionType, ComponentInfo> ComponentInfoAction; 
  
  private string _cachedName;
  
  public void Initialize(IContext context, IEntity entity)
  {
    _context = context;
    _entity = entity;
    
    // component changed 
    _entity.OnComponentAdded += OnComponentAdded;
    _entity.OnComponentReplaced += OnComponentReplaced;
    _entity.OnComponentRemoved += OnComponentRemoved;
    
    RefreshName();
  }

  private void OnComponentRemoved(IEntity entity, int index, IComponent component)
  {
    if (_componentInfos.TryGetValue(component.GetType(), out ComponentInfo componentInfo))
    {
      _componentInfos.Remove(component.GetType());
      
      _sortedComponentInfos.Clear();
      _sortedComponentInfos.AddRange(_componentInfos.Values.OrderBy(info => info.Index));
      
      ComponentInfoAction?.Invoke(ComponentActionType.Removed, componentInfo);
    }
  
    RefreshName();
  }
  
  private void OnComponentReplaced(IEntity entity, int index, IComponent previousComponent, IComponent newComponent)
  {
    if (_componentInfos.TryGetValue(previousComponent.GetType(), out ComponentInfo componentInfo))
    {
      componentInfo.UpdateComponent(newComponent);
      ComponentInfoAction?.Invoke(ComponentActionType.Replaced, componentInfo);
    }
    RefreshName();
  }

  private void OnComponentAdded(IEntity entity, int index, IComponent component)
  {
    ComponentInfo componentInfo = new(entity, index, component);
    
    _componentInfos.Add(component.GetType(), componentInfo);
      
    _sortedComponentInfos.Clear();
    _sortedComponentInfos.AddRange(_componentInfos.Values.OrderBy(info => info.Index));
      
    ComponentInfoAction?.Invoke(ComponentActionType.Added, componentInfo);
    RefreshName();
  }
  
  private void RefreshName()
  {
    if (_cachedName != _entity.ToString())
      Name = _cachedName = _entity.ToString();
  }
  
  public void CleanUp()
  {
    _entity.OnComponentAdded -= OnComponentAdded;
    _entity.OnComponentReplaced -= OnComponentReplaced;
    _entity.OnComponentRemoved -= OnComponentRemoved;

    ComponentInfoAction = null;
    Name = "Enity";

    _entity = null;
    _context = null;
  }
}