using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

public partial class ContextInspector : BaseInspector
{
  [Export] private Container _entitiesContainer;
  [Export] private Label _reusableCount;
  [Export] private Container _groupContainer;

  private readonly Dictionary<IEntity, Label> _entities = new();
  private readonly Dictionary<IGroup, Label> _groups = new();
  
  private ContextObserverNode _contextObserverNode;
  private IContext _context;

  public override void Initialize(Node node)
  {
    if (node == _contextObserverNode) return;
    _contextObserverNode = (ContextObserverNode)node;
    _context = _contextObserverNode.Context;
    _context.OnEntityCreated += OnEntityCreated;
    _context.OnEntityDestroyed += OnEntityDestroyed;
    _context.OnGroupCreated += OnGroupCreated;
    
    foreach (IEntity entity in _context.Entities())
      CreateEntityLabel(entity);
    
    _reusableCount.Text = $"[{_contextObserverNode.Context.reusableEntitiesCount}]";
    
    // foreach (IGroup<IEntity> group in _context.Groups())
    //   CreateGroupLabel(group);
  }

  public override void CleanUp()
  {
    _context.OnEntityCreated -= OnEntityCreated;
    _context.OnEntityDestroyed -= OnEntityDestroyed;
    _context.OnGroupCreated -= OnGroupCreated;

    foreach (Node child in _entitiesContainer.GetChildren())
    {
      _entitiesContainer.RemoveChild(child);
      child.QueueFree();
    }

    foreach (Node child in _groupContainer.GetChildren())
    {
      _groupContainer.RemoveChild(child);
      child.QueueFree();
    }

    _entities.Clear();
    _groups.Clear();
    
    _contextObserverNode = null;
    _context = null;
  }

  private void CreateGroupLabel(IGroup group)
  {
    Label label = new Label();
    label.Text = group.ToString();
    _groupContainer.AddChild(label);
    _groups.Add(group, label);
  }
  
  private void CreateEntityLabel(IEntity entity)
  {
    Label label = new Label();
    label.Text = entity.ToString();
    _entitiesContainer.AddChild(label);
    _entities.Add(entity, label);
  }
  
  private void OnGroupCreated(IContext context, IGroup group) => CreateGroupLabel(group);

  private void OnEntityDestroyed(IContext context, IEntity entity)
  {
    if (_entities.Remove(entity, out Label label))
    {
      _entitiesContainer.RemoveChild(label);
      label.QueueFree();
    }
  }

  private void OnEntityCreated(IContext context, IEntity entity) => CreateEntityLabel(entity);
}