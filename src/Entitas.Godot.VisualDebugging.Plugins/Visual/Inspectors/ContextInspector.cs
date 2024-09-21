using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

public partial class ContextInspector : BaseInspector
{
  private Dictionary<IEntity, Label> _entities = new();
  private Dictionary<IGroup, Label> _groups = new();
  private Container _entitiesContainer;
  private Label _reusable;
  private Container _groupContainer;
  
  private ContextObserverNode _contextObserverNode;
  private IContext _context;

  public override void Initialize(Node node)
  {
    if (node == _contextObserverNode) return;
    _contextObserverNode = (ContextObserverNode)node;
    _context = _contextObserverNode.Context;
    
    AddThemeConstantOverride("margin_top", Consts.Margin);
    AddThemeConstantOverride("margin_left", Consts.Margin);
    AddThemeConstantOverride("margin_bottom", Consts.Margin);
    AddThemeConstantOverride("margin_right", Consts.Margin);
    LayoutMode = 2;
    AnchorsPreset = (int) LayoutPreset.FullRect;
    AnchorRight = 1;
    AnchorBottom = 1;
    GrowHorizontal = GrowDirection.Both;
    GrowVertical = GrowDirection.Both;
    SizeFlagsHorizontal = SizeFlags.ExpandFill;

    VBoxContainer content = new();
    content.LayoutMode = 2;
    content.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    AddChild(content);
    
    content.AddChild(CreateTitleLabel("Entities:"));

    _entitiesContainer = new VBoxContainer();
    _entitiesContainer.AddThemeConstantOverride("separation", Consts.Margin);
    _entitiesContainer.LayoutMode = 2;
    _entitiesContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    content.AddChild(_entitiesContainer);
    
    foreach (IEntity entity in _context.Entities())
    {
      Label label = CreateContentLabel(entity.ToString());
      _entitiesContainer.AddChild(label);
      _entities.Add(entity, label);
    }
    
    HBoxContainer reusableContainer = new HBoxContainer();
    reusableContainer.AddThemeConstantOverride("separation", Consts.Margin);
    reusableContainer.LayoutMode = 2;
    reusableContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    content.AddChild(reusableContainer);
    
    reusableContainer.AddChild(CreateTitleLabel("Reusable:"));
    _reusable = CreateContentLabel($"[{_contextObserverNode.Context.reusableEntitiesCount}]");
    reusableContainer.AddChild(_reusable);
    
    content.AddChild(CreateTitleLabel("Group:"));
    _groupContainer = new VBoxContainer();
    _groupContainer.AddThemeConstantOverride("separation", Consts.Margin);
    _groupContainer.LayoutMode = 2;
    _groupContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    content.AddChild(_groupContainer);

    // foreach (IGroup<IEntity> group in _context.Groups())
    //   CreateGroupLabel(group);
    
    _context.OnEntityCreated += OnEntityCreated;
    _context.OnEntityDestroyed += OnEntityDestroyed;
    _context.OnGroupCreated += OnGroupCreated;
  }

  private Label CreateTitleLabel(string text)
  {
    Label label = new Label();
    label.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    label.Text = text;
    return label;
  }
  
  private Label CreateContentLabel(string text)
  {
    Label label = new Label();
    label.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    label.Text = text;
    return label;
  }

  public override void CleanUp()
  {
    _context.OnEntityCreated -= OnEntityCreated;
    _context.OnEntityDestroyed -= OnEntityDestroyed;
    _context.OnGroupCreated -= OnGroupCreated;

    foreach (Label entityLabel in _entities.Values)
      entityLabel.QueueFree();
    
    foreach (Label groupLabel in _groups.Values)
      groupLabel.QueueFree();
    
    foreach (Node child in GetChildren())
      child.QueueFree();
    
    _entitiesContainer.QueueFree();
    _reusable.QueueFree();
    _groupContainer.QueueFree();
    
    _entities.Clear();
    _groups.Clear();

    _entitiesContainer = null;
    _reusable = null;
    _groupContainer = null;
    _contextObserverNode = null;
    _context = null;
  }
  
  private void OnGroupCreated(IContext context, IGroup group)
  {
    Label label = CreateContentLabel(group.ToString());
    _groups.Add(group, label);
    _groupContainer.AddChild(label);
    
    _reusable.Text = $"[{_contextObserverNode.Context.reusableEntitiesCount}]";
  }

  private void OnEntityDestroyed(IContext context, IEntity entity)
  {
    if (_entities.Remove(entity, out Label label))
    {
      _entitiesContainer.RemoveChild(label);
      label.QueueFree();
      
      _reusable.Text = $"[{_contextObserverNode.Context.reusableEntitiesCount}]";
    }
  }

  private void OnEntityCreated(IContext context, IEntity entity)
  {
    Label label = CreateContentLabel(entity.ToString());
    _entities.Add(entity, label);
    _entitiesContainer.AddChild(label);
  }
}