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
  
  private IContext _context;

  public void Initialize(IContext context)
  {
    if (_context == context) return;
    _context = context;

    InitializeTree();
    
    foreach (IEntity entity in _context.Entities())
    {
      Label label = CreateContentLabel(entity.ToString());
      _entitiesContainer.AddChild(label);
      _entities.Add(entity, label);
    }

    _reusable.Text = $"[{_context.reusableEntitiesCount}]";
    
    // foreach (IGroup<IEntity> group in _context.Groups())
    //   CreateGroupLabel(group);
    
    _context.OnEntityCreated += OnEntityCreated;
    _context.OnEntityDestroyed += OnEntityDestroyed;
    _context.OnGroupCreated += OnGroupCreated;
  }

  private void InitializeTree()
  {
    if (GetChildCount() > 0) return;
    
    AddThemeConstantOverride("margin_top", Consts.Margin);
    AddThemeConstantOverride("margin_left", Consts.Margin);
    AddThemeConstantOverride("margin_bottom", Consts.Margin);
    AddThemeConstantOverride("margin_right", Consts.Margin);
    AnchorsPreset = (int) LayoutPreset.FullRect;
    GrowHorizontal = GrowDirection.Both;
    GrowVertical = GrowDirection.Both;
    SizeFlagsHorizontal = SizeFlags.ExpandFill;

    VBoxContainer content = new();
    content.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    AddChild(content);
    
    content.AddChild(CreateTitleLabel("Entities:"));

    _entitiesContainer = new VBoxContainer();
    _entitiesContainer.AddThemeConstantOverride("separation", Consts.Margin);
    _entitiesContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    content.AddChild(_entitiesContainer);
    
    HBoxContainer reusableContainer = new();
    reusableContainer.AddThemeConstantOverride("separation", Consts.Margin);
    reusableContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    content.AddChild(reusableContainer);
    
    reusableContainer.AddChild(CreateTitleLabel("Reusable:"));
    reusableContainer.AddChild(_reusable = CreateContentLabel("[0]"));
    
    content.AddChild(CreateTitleLabel("Group:"));
    _groupContainer = new VBoxContainer();
    _groupContainer.AddThemeConstantOverride("separation", Consts.Margin);
    _groupContainer.LayoutMode = 2;
    _groupContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    content.AddChild(_groupContainer);
  }

  private Label CreateTitleLabel(string text)
  {
    Label label = new();
    label.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    label.Text = text;
    return label;
  }
  
  private Label CreateContentLabel(string text)
  {
    Label label = new();
    label.AddThemeFontSizeOverride("font_size", Consts.FontContentSize);
    label.Text = text;
    return label;
  }

  public override void CleanUp()
  {
    if (_context == null) return;
    
    _context.OnEntityCreated -= OnEntityCreated;
    _context.OnEntityDestroyed -= OnEntityDestroyed;
    _context.OnGroupCreated -= OnGroupCreated;

    foreach (Label entityLabel in _entities.Values)
      entityLabel.QueueFree();
    
    foreach (Label groupLabel in _groups.Values)
      groupLabel.QueueFree();
    
    _entities.Clear();
    _groups.Clear();
    
    _context = null;
  }
  
  private void OnGroupCreated(IContext context, IGroup group)
  {
    Label label = CreateContentLabel(group.ToString());
    _groups.Add(group, label);
    _groupContainer.AddChild(label);
    
    _reusable.Text = $"[{_context.reusableEntitiesCount}]";
  }

  private void OnEntityDestroyed(IContext context, IEntity entity)
  {
    if (_entities.Remove(entity, out Label label))
    {
      _entitiesContainer.RemoveChild(label);
      label.QueueFree();
      
      _reusable.Text = $"[{_context.reusableEntitiesCount}]";
    }
  }

  private void OnEntityCreated(IContext context, IEntity entity)
  {
    Label label = CreateContentLabel(entity.ToString());
    _entities.Add(entity, label);
    _entitiesContainer.AddChild(label);
    
    _reusable.Text = $"[{_context.reusableEntitiesCount}]";
  }
}