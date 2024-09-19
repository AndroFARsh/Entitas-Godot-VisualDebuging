namespace Entitas.Godot.VisualDebugging.Plugins.tests;

public class RootFeature : Feature
{
  public RootFeature()
  {
    Add(new ChildFeature());
  }
}

public class ChildFeature : Feature
{
  public ChildFeature()
  {
    Add(new UpdateRotationSystem());
    Add(new RotationToNode2dSystem());
  }
}

public class UpdateRotationSystem : IExecuteSystem
{
  private readonly IGroup<GameEntity> _entities;

  public UpdateRotationSystem()
  {
    GameContext game = Contexts.sharedInstance.game;
    _entities = game.GetGroup(GameMatcher
      .AllOf(GameMatcher.RotationSpeed, GameMatcher.Rotation));
  }

  public void Execute()
  {
    foreach (GameEntity entity in _entities)
    {
      entity.ReplaceRotation((entity.Rotation + entity.RotationSpeed * TimeService.DeltaTime) % 360);
    }
  }
}

public class RotationToNode2dSystem : IExecuteSystem
{
  private readonly IGroup<GameEntity> _entities;

  public RotationToNode2dSystem()
  {
    GameContext game = Contexts.sharedInstance.game;
    _entities = game.GetGroup(GameMatcher
      .AllOf(GameMatcher.Node2D, GameMatcher.Rotation));
  }

  public void Execute()
  {
    foreach (GameEntity entity in _entities)
    {
      if (entity.Node2D != null)
      {
        entity.Node2D.Rotation = entity.Rotation;
      }
    }
  }
}