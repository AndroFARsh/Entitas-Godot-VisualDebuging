using System;
using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

public static class DrawerFactory
{
  private static readonly Dictionary<Type, Func<BaseValueDrawer>> _factories = new();

  static DrawerFactory()
  {
    _factories.Add(typeof(bool), () => new BoolValueDrawer());
    _factories.Add(typeof(int), () => new IntValueDrawer());
    _factories.Add(typeof(long), () => new LongValueDrawer());
    _factories.Add(typeof(float), () => new FloatValueDrawer());
    _factories.Add(typeof(double), () => new DoubleValueDrawer());
    _factories.Add(typeof(string), () => new StringValueDrawer());
    _factories.Add(typeof(Enum), () => new EnumValueDrawer());
    _factories.Add(typeof(Vector2), () => new Vector2ValueDrawer());
    _factories.Add(typeof(Vector2I), () => new Vector2IValueDrawer());
    _factories.Add(typeof(Rect2), () => new Rect2ValueDrawer());
    _factories.Add(typeof(Rect2I), () => new Rect2IValueDrawer());
    _factories.Add(typeof(Vector3), () => new Vector3ValueDrawer());
    _factories.Add(typeof(Vector3I), () => new Vector3IValueDrawer());
    _factories.Add(typeof(Transform2D), () => new Transform2DValueDrawer());
    _factories.Add(typeof(Vector4), () => new Vector4ValueDrawer());
    _factories.Add(typeof(Vector4I), () => new Vector4IValueDrawer());
    _factories.Add(typeof(Plane), () => new PlaneValueDrawer());
    _factories.Add(typeof(Quaternion), () => new QuaternionValueDrawer());
    _factories.Add(typeof(Aabb), () => new AabbValueDrawer());
    _factories.Add(typeof(Basis), () => new BasisValueDrawer());
    _factories.Add(typeof(Transform3D), () => new Transform3DValueDrawer());
    _factories.Add(typeof(Projection), () => new ProjectionValueDrawer());
    _factories.Add(typeof(Color), () => new ColorValueDrawer());
    _factories.Add(typeof(StringName), () => new StringNameValueDrawer());
    _factories.Add(typeof(NodePath), () => new NodePathValueDrawer());
  }

  public static BaseValueDrawer CreateDrawer(this Type type)
  {
    if (_factories.TryGetValue(type, out var factory))
      return factory.Invoke();
    if (type.IsEnum)
      return new EnumValueDrawer();
    
    return new ObjectValueDrawer();
  }

  public static BaseValueDrawer CreateDrawer(this object value) => value switch
  {
    bool => new BoolValueDrawer(),
    int => new IntValueDrawer(),
    long => new LongValueDrawer(),
    float => new FloatValueDrawer(),
    double => new DoubleValueDrawer(),
    string => new StringValueDrawer(),
    Enum => new EnumValueDrawer(),
    Vector2 => new Vector2ValueDrawer(),
    Vector2I => new Vector2IValueDrawer(),
    Rect2 => new Rect2ValueDrawer(),
    Rect2I => new Rect2IValueDrawer(),
    Vector3 => new Vector3ValueDrawer(),
    Vector3I => new Vector3IValueDrawer(),
    Transform2D => new Transform2DValueDrawer(),
    Vector4 => new Vector4ValueDrawer(),
    Vector4I => new Vector4IValueDrawer(),
    Plane => new PlaneValueDrawer(),
    Quaternion => new QuaternionValueDrawer(),
    Aabb => new AabbValueDrawer(),
    Basis => new BasisValueDrawer(),
    Transform3D => new Transform3DValueDrawer(),
    Projection => new ProjectionValueDrawer(),
    Color => new ColorValueDrawer(),
    StringName => new StringNameValueDrawer(),
    NodePath => new NodePathValueDrawer(),

    // Dictionary dictionaryValue => Variant.CreateFrom(dictionaryValue),
    // Array arrayValue => Variant.CreateFrom(arrayValue),
    // byte[] byteArrayValue => Variant.CreateFrom(byteArrayValue),
    // int[] intArrayValue => Variant.CreateFrom(intArrayValue),
    // long[] longArrayValue => Variant.CreateFrom(longArrayValue),
    // float[] floatArrayValue => Variant.CreateFrom(floatArrayValue),
    // double[] doubleArrayValue => Variant.CreateFrom(doubleArrayValue),
    // string[] stringArrayValue => Variant.CreateFrom(stringArrayValue),
    // Vector2[] vector2ArrayValue => Variant.CreateFrom(vector2ArrayValue),
    // Vector3[] vector3ArrayValue => Variant.CreateFrom(vector3ArrayValue),
    // Color[] colorArrayValue => Variant.CreateFrom(colorArrayValue),

    _ => new ObjectValueDrawer()
  };
}