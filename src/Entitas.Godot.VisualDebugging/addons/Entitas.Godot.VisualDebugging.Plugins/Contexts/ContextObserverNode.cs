using System;
using System.Collections.Generic;
using System.Text;
using Godot;

namespace Entitas.Godot;

public enum EntityActionType
{
  Created,
  Destroyed
}

public partial class ContextObserverNode : Node
{
  private readonly List<IGroup> _groups = new();
  
  private readonly Dictionary<IEntity, EntityObserverNode> _entities = new();
  private readonly StringBuilder _toStringBuilder = new();

  private IContext _context;
  
  public IContext Context => _context;
  public ContextInfo ContextInfo => _context.contextInfo;
  
  public event Action<ContextObserverNode> OnNameChanged;
  public event Action<EntityActionType, ContextObserverNode, EntityObserverNode> OnEntityChanged;

  public void Initialize(IContext context)
  {
    _context = context;
    _context.OnEntityCreated += OnEntityCreated;
    _context.OnEntityDestroyed += OnEntityDestroyed;
    _context.OnGroupCreated += OnGroupCreated;
    Name = ToString();
  }

  private void OnEntityCreated(IContext context, IEntity entity)
  {
    if (!EntitasRoot.Pool.Request(typeof(EntityObserverNode), out EntityObserverNode entityObserverNode))
      entityObserverNode = new EntityObserverNode();
    
    _entities.Add(entity, entityObserverNode);
    
    entityObserverNode.Initialize(context, entity);
    AddChild(entityObserverNode);
    
    Name = ToString();
    OnNameChanged?.Invoke(this);
    OnEntityChanged?.Invoke(EntityActionType.Created, this, entityObserverNode);
  }
  
  private void OnEntityDestroyed(IContext context, IEntity entity)
  {
    Name = ToString();
    OnNameChanged?.Invoke(this);

    if (_entities.TryGetValue(entity, out EntityObserverNode entityObserver))
    {
      OnEntityChanged?.Invoke(EntityActionType.Destroyed, this, entityObserver);
      
      entityObserver.CleanUp();
      RemoveChild(entityObserver);

      EntitasRoot.Pool.Retain(typeof(EntityObserverNode), entityObserver);
    }
  }
  
  private void OnGroupCreated(IContext context, IGroup group)
  {
    _groups.Add(group);
    Name = ToString();
    OnNameChanged?.Invoke(this);
  } 
  
  protected override void Dispose(bool disposing)
  {
    _entities.Clear();
    
    _context.OnEntityCreated -= OnEntityCreated;
    _context.OnEntityDestroyed -= OnEntityDestroyed;
    _context.OnGroupCreated -= OnGroupCreated;
    _context = null;
    
    base.Dispose(disposing);
  }

  public override string ToString()
  {
    _toStringBuilder.Length = 0;
    _toStringBuilder
      .Append(_context.contextInfo.name).Append(" (")
      .Append(_context.count).Append(" entities, ")
      .Append(_context.reusableEntitiesCount).Append(" reusable, ");

    if (_context.reusableEntitiesCount != 0)
    {
      _toStringBuilder.Append(_context.reusableEntitiesCount).Append(" retained, ");
    }

    _toStringBuilder.Append(_groups.Count).Append(" groups)");

    return _toStringBuilder.ToString();
  }
}
