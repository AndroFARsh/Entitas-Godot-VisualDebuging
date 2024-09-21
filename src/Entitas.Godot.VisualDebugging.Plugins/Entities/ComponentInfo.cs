using System;
using System.Linq;
using System.Reflection;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

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

  public Variant GetFieldValueAsVariant(string fieldName)
  {
    object value = GetFieldValue(fieldName);
    return value switch
    {
      bool boolValue => Variant.CreateFrom(boolValue),
      int intValue => Variant.CreateFrom(intValue),
      long longValue => Variant.CreateFrom(longValue),
      float floatValue => Variant.CreateFrom(floatValue),
      double doubleValue => Variant.CreateFrom(doubleValue),
      string stringValue => Variant.CreateFrom(stringValue),
      Vector2 vector2Value => Variant.CreateFrom(vector2Value),
      Vector2I vector2IValue => Variant.CreateFrom(vector2IValue),
      Rect2 rect2Value => Variant.CreateFrom(rect2Value),
      Rect2I rect2IValue => Variant.CreateFrom(rect2IValue),
      Vector3 vector3Value => Variant.CreateFrom(vector3Value),
      Vector3I vector3IValue => Variant.CreateFrom(vector3IValue),
      Transform2D transform2DValue => Variant.CreateFrom(transform2DValue),
      Vector4 vector4Value => Variant.CreateFrom(vector4Value),
      Vector4I vector4IValue => Variant.CreateFrom(vector4IValue),
      Plane planeValue => Variant.CreateFrom(planeValue),
      Quaternion quaternionValue => Variant.CreateFrom(quaternionValue),
      Aabb aabbValue => Variant.CreateFrom(aabbValue),
      Basis basisValue => Variant.CreateFrom(basisValue),
      Transform3D transform3DValue => Variant.CreateFrom(transform3DValue),
      Projection projectionValue => Variant.CreateFrom(projectionValue),
      Color colorValue => Variant.CreateFrom(colorValue),
      StringName stringNameValue => Variant.CreateFrom(stringNameValue),
      NodePath nodePathValue => Variant.CreateFrom(nodePathValue),
      Rid ridValue => Variant.CreateFrom(ridValue),
      GodotObject godotObjectValue => Variant.CreateFrom(godotObjectValue),
      Callable callableValue => Variant.CreateFrom(callableValue),
      Signal signalValue => Variant.CreateFrom(signalValue),
      Dictionary dictionaryValue => Variant.CreateFrom(dictionaryValue),
      Array arrayValue => Variant.CreateFrom(arrayValue),
      byte[] byteArrayValue => Variant.CreateFrom(byteArrayValue),
      int[] intArrayValue => Variant.CreateFrom(intArrayValue),
      long[] longArrayValue => Variant.CreateFrom(longArrayValue),
      float[] floatArrayValue => Variant.CreateFrom(floatArrayValue),
      double[] doubleArrayValue => Variant.CreateFrom(doubleArrayValue),
      string[] stringArrayValue => Variant.CreateFrom(stringArrayValue),
      Vector2[] vector2ArrayValue => Variant.CreateFrom(vector2ArrayValue),
      Vector3[] vector3ArrayValue => Variant.CreateFrom(vector3ArrayValue),
      Color[] colorArrayValue => Variant.CreateFrom(colorArrayValue),
      _ => default
    };
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
    {
      fieldInfo.SetValue(Component, newValue);
    }
  }

  public void SetFieldValueFromVariant(string fieldName, Variant newValue) =>
    SetFieldValue(fieldName, newValue.Obj);
}