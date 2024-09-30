using System;
using System.Linq;
using System.Reflection;

namespace Entitas.Godot;

public class ComponentInfo
{
  public Type Type { get; private set; }
  public string Name { get; private set; }
  public IEntity Entity { get; private set; }
  public int Index { get; private set; }
  public IComponent Component { get; private set; }
  
  public bool IsFlaggableComponent => _fields.Count == 0;
  
  public System.Collections.Generic.IEnumerable<string> FieldNames => _fields.Keys;

  private System.Collections.Generic.Dictionary<string, FieldInfo> _fields;
  
  public ComponentInfo(IEntity entity, int index, IComponent component)
  {
    Type = entity.contextInfo.componentTypes[index];
    Name = entity.contextInfo.componentNames[index];
    Entity = entity;
    Index = index;
    UpdateComponent(component);
  }

  public void UpdateComponent(IComponent component)
  {
    Component = component;
    _fields = component
      .GetType()
      .GetFields()
      .ToDictionary(info => info.Name);
  }

  public T GetFieldValue<T>(string fieldName) => (T)GetFieldValue(fieldName);

  public Type GetFieldType(string fieldName)
  {
    return _fields.TryGetValue(fieldName, out FieldInfo fieldInfo)
      ? fieldInfo.FieldType
      : default;
  }

  public object GetFieldValue(string fieldName)
  {
    return _fields.TryGetValue(fieldName, out FieldInfo fieldInfo)
     ? fieldInfo.GetValue(Component)
     : default;
  }

  public void SetFieldValue(string fieldName, object newValue)
  {
    if (_fields.TryGetValue(fieldName, out FieldInfo fieldInfo))
      fieldInfo.SetValue(Component, newValue);
  }
}