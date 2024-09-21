using Entitas.Godot.VisualDebugging.Plugins.tests;
using Godot;

namespace Entitas.Godot;

class TimeService
{
  public static float DeltaTime { get; set; }
}

[GlobalClass]
public partial class TestProject : Node
{
  [Export] private Node2D _icon;
  
  private RootFeature _feature;

  public override void _Ready()
  {
    MetaEntity metaEntity = Contexts.sharedInstance.meta.CreateEntity();
    metaEntity.isFlag = true;
    
    Contexts.sharedInstance.game.CreateEntity()
      .AddFloatType(2.2f)
      .AddIntType(5)
      .AddVector2Type(new Vector2(0.5f, 0.5f))
      ;

    Contexts.sharedInstance.game.CreateEntity()
      .AddEnumValues(
        ValueTestEnum.Value2,
        FlagTestEnum.Flag4
      );

    Contexts.sharedInstance.game.CreateEntity()
      .AddMultiValues1(
        1,
        2,
        3.5f,
        4.5,
        "test",
        new Vector2(0.5f, 1.5f),
        new Vector2I(1, 2),
        new Rect2(0.5f, 1.5f, 1f, 2f),
        new Rect2I(1, 2, 3, 4));
    
    Contexts.sharedInstance.game.CreateEntity()
      .AddMultiValues2(
        new Vector3(1, 2, 3.5f),
        new Vector3I(1, 2, 3),
        new Transform2D(1, new Vector2(2, 3)),
        new Vector4(1, 2, 3, 4),
        new Vector4I(1, 2, 3, 4),
        new Plane(new Vector3(0, 1, 0), new Vector3(1, 1, 0)),
        new Quaternion(0.0f, 0.0f, 0.0f, 1f))
      .isFlag = true;
    
    Contexts.sharedInstance.game.CreateEntity()
      .AddMultiValues3(
        new Aabb(new Vector3(1,1,1), new Vector3(2,2,2)),
        Basis.Identity,
        Transform3D.Identity,
        Projection.Identity,
        new Color(1f, 0, 0),
        new StringName("Test"),
        new NodePath("res://Scenes/project.tscn")
       );

    Contexts.sharedInstance.game.CreateEntity()
      .AddNode2D(_icon)
      .AddRotation(0)
      .AddRotationSpeed(3)
      ;
    
    _feature = new RootFeature();
    _feature.Initialize();
  }

  public override void _Process(double delta)
  {
    TimeService.DeltaTime = (float) delta;
    _feature.Execute();
    _feature.Cleanup();
  }

  protected override void Dispose(bool disposing)
  {
    _feature.TearDown();
    _feature = null;
    base.Dispose(disposing);
  }
}