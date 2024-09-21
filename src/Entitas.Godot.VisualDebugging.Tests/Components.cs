using Godot;

namespace Entitas.Godot;

public enum ValueTestEnum : long
{
  Value0,
  Value1,
  Value2,
  Value3
}

public enum FlagTestEnum : byte
{
  Flag1 = 1,
  Flag2 = 2,
  Flag4 = 4,
  Flag8 = 8
}

[Game, Meta] public class FlagComponent : IComponent { }

[Game] public class IntTypeComponent : IComponent { public int Value; }

[Game] public class FloatTypeComponent : IComponent { public float Value; }

[Game] public class Vector2TypeComponent : IComponent { public Vector2 Value; }

[Game] public class Node2DComponent : IComponent { public Node2D Value; }

[Game] public class RotationComponent : IComponent { public float Value; }

[Game] public class RotationSpeedComponent : IComponent { public float Value; }

[Game]
public class EnumValuesComponent : IComponent
{
  public ValueTestEnum ValueEnum;
  public FlagTestEnum FlagEnum;
}

[Game]
public class MultiValues1Component : IComponent
{
  public int IntValue;
  public long LongValue;
  public float FloatValue;
  public double DoubleValue;
  public string StringValue;
  public Vector2 Vector2Value;
  public Vector2I Vector2IValue;
  public Rect2 Rect2Value;
  public Rect2I Rect2IValue;
}

[Game]
public class MultiValues2Component : IComponent
{
  public Vector3 Vector3Value;
  public Vector3I Vector3IValue;
  public Transform2D Transform2DValue;
  public Vector4 Vector4Value;
  public Vector4I Vector4IValue;
  public Plane PlaneValue;
  public Quaternion QuaternionValue;
}

[Game]
public class MultiValues3Component : IComponent
{
  public Aabb AabbValue;
  public Basis BasisValue;
  public Transform3D Transform3DValue;
  public Projection ProjectionValue;
  public Color ColorValue;
  public StringName StringNameValue;
  public NodePath NodePathValue;
}