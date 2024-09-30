using System;
using System.Collections.Generic;
using Godot;

namespace Entitas.Godot;

public partial class EntityInspector : BaseInspector
{
  private Button _destroyEntityButton;
  private Button _cloneEntityButton;
  private OptionButton _componentsButton;
  private TextEdit _searchTextEdit;
  private Button _clearSearchButton;
  private Container _componentsContainer;

  private ContextInfo _contextInfo;
  private IEntity _entity;
  private IContext _context;
  
  private readonly Dictionary<IComponent, ComponentDrawer> _drawers = new();

  public void Initialize(IEntity entity)
  {
    if (_entity == entity) return;
    
    _entity = entity;
    _context = EntitasRoot.Global.GetContext(entity);
    _contextInfo = _context.contextInfo;
    
    InitializeTree();

    _entity.OnComponentAdded += OnComponentAdded;
    _entity.OnComponentRemoved += OnComponentRemoved;
    
    _componentsButton.ItemSelected += OnComponentAddAction;
    _destroyEntityButton.Pressed += OnDestroyEntityPressed;
    _cloneEntityButton.Pressed += OnCloneEntityPressed;
    _searchTextEdit.TextChanged += OnSearchTextChanged;
    _clearSearchButton.Pressed += OnClearSearchPressed;

    FillComponentDropDown();

    foreach (int index in _entity.GetComponentIndices())
    {
      IComponent component = _entity.GetComponent(index);
      ComponentInfo componentInfo = new(entity, index, component);
      ComponentDrawer componentDrawer = CreateComponentDrawer(_entity, componentInfo);
      _componentsContainer.AddChild(componentDrawer);
      _drawers.Add(component, componentDrawer);
    }

    Visible = true;
  }

  private void OnComponentRemoved(IEntity entity, int index, IComponent component)
  {
    if (_drawers.TryGetValue(component, out ComponentDrawer drawer))
    {
      drawer.CleanUp();
      EntitasRoot.Global.Pool.Retain(typeof(ComponentInfo), drawer);
      _drawers.Remove(component);
      _componentsContainer.RemoveChild(drawer);

      FillComponentDropDown();
    }
  }
  
  private void OnComponentAdded(IEntity entity, int index, IComponent component)
  {
    OnComponentRemoved(entity, index, component);
    
    ComponentInfo componentInfo = new(entity, index, component);
    ComponentDrawer componentDrawer = CreateComponentDrawer(entity, componentInfo);
    _componentsContainer.AddChild(componentDrawer);
    
    _drawers.Add(component, componentDrawer);

    FillComponentDropDown();
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
    content.AddThemeConstantOverride("separation", Consts.Margin);
    content.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    AddChild(content);
    
    _destroyEntityButton = new Button();
    _destroyEntityButton.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    _destroyEntityButton.Text = "Destroy Entity";
    content.AddChild(_destroyEntityButton);
    
    _cloneEntityButton = new Button();
    _cloneEntityButton.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    _cloneEntityButton.Text = "Clone Entity";
    content.AddChild(_cloneEntityButton);

    HBoxContainer addComponentContent = new();
    addComponentContent.AddThemeConstantOverride("separation", Consts.Margin);
    content.AddChild(addComponentContent);

    Label addComponentTitle = new();
    addComponentTitle.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    addComponentTitle.Text = "Add Component";
    addComponentContent.AddChild(addComponentTitle);
    
    _componentsButton = new OptionButton();
    _componentsButton.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    _componentsButton.CustomMinimumSize = new Vector2(0, 50);
    _componentsButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    _componentsButton.FitToLongestItem = false;
    addComponentContent.AddChild(_componentsButton);
    
    HBoxContainer searchContent = new();
    searchContent.AddThemeConstantOverride("separation", Consts.Margin);
    content.AddChild(searchContent);

    Label searchContentIcon = new Label();
    searchContentIcon.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    searchContentIcon.Text = "\ud83d\udd0d";
    searchContent.AddChild(searchContentIcon);

    _searchTextEdit = new TextEdit();
    _searchTextEdit.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    _searchTextEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    _searchTextEdit.ShortcutKeysEnabled = false;
    _searchTextEdit.SelectingEnabled = true;
    _searchTextEdit.ScrollFitContentHeight = true;
    searchContent.AddChild(_searchTextEdit);
    
    _clearSearchButton = new Button();
    _clearSearchButton.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    _clearSearchButton.Text = "\u26cc";
    _clearSearchButton.Size = new Vector2I(50, 50);
    searchContent.AddChild(_clearSearchButton);

    _componentsContainer = new VBoxContainer();
    _componentsContainer.AddThemeConstantOverride("separation", Consts.Margin);
    content.AddChild(_componentsContainer);
  }

  private void FillComponentDropDown()
  {
    _componentsButton.Clear();
    if (_entity == null) return;

    int index = 0;
    foreach (Type componentType in _contextInfo.componentTypes)
    {
      if (Array.IndexOf(_contextInfo.componentTypes, componentType) >= 0)
      {
        _componentsButton.AddItem(componentType.FullName, index);
      }

      index++;
    }

    _componentsButton.Selected = -1;
  }

  private ComponentDrawer CreateComponentDrawer(IEntity entity, ComponentInfo componentInfo)
  {
    if (!EntitasRoot.Global.Pool.Request(typeof(ComponentInfo), out ComponentDrawer drawer))
      drawer = new ComponentDrawer();

    drawer.Initialize(entity, componentInfo);
    return drawer;
  }

  public override void CleanUp()
  {
    if (_entity == null) return;

    _componentsButton.Clear();
    foreach ((IComponent _, ComponentDrawer drawer) in _drawers)
    {
      drawer.CleanUp();
      
      _componentsContainer.RemoveChild(drawer);

      EntitasRoot.Global.Pool.Retain(typeof(ComponentInfo), drawer);
    }

    _drawers.Clear();

    _componentsButton.ItemSelected -= OnComponentAddAction;
    _destroyEntityButton.Pressed -= OnDestroyEntityPressed;
    _cloneEntityButton.Pressed -= OnCloneEntityPressed;
    _searchTextEdit.TextChanged -= OnSearchTextChanged;
    _clearSearchButton.Pressed -= OnClearSearchPressed;

    _entity.OnComponentAdded -= OnComponentAdded;
    _entity.OnComponentRemoved -= OnComponentRemoved;
    
    _entity = null;
    _contextInfo = null;
    Visible = false;
  }

  private void OnClearSearchPressed()
  {
    _searchTextEdit.Text = "";
    OnSearchTextChanged();
  }

  private void OnDestroyEntityPressed() => _entity.Destroy();

  private void OnSearchTextChanged()
  {
    foreach (Node node in _componentsContainer.GetChildren())
      if (node is ComponentDrawer componentNode)
      {
        ComponentInfo componentInfo = componentNode.ComponentInfo;
        string componentName = componentInfo?.Name ?? "";
        componentNode.Visible = componentName.Contains(_searchTextEdit.Text, StringComparison.CurrentCultureIgnoreCase);
      }
  }

  private void OnCloneEntityPressed()
  {
    IEntity newEntity = _context.CreateEntity();
    foreach (int index in _entity.GetComponentIndices())
    {
      IComponent component = _entity.GetComponent(index);
      ComponentInfo componentInfo = new(newEntity, index, component);
      
      IComponent newComponent = Activator.CreateInstance(componentInfo.Type) as IComponent;
      ComponentInfo newComponentInfo = new(newEntity, index, newComponent);

      foreach (string fieldName in componentInfo.FieldNames)
      {
        object value = componentInfo.GetFieldValue(fieldName);
        newComponentInfo.SetFieldValue(fieldName, value);
      }

      try
      {
        newEntity.AddComponent(index, newComponent);
      }
      catch (Exception e)
      {
        GD.PrintErr(e);
      }
    }
  }

  private void OnComponentAddAction(long i)
  {
    int index = _componentsButton.GetItemId((int)i);
    if (!_entity.HasComponent(index))
    {
      Type componentType = _contextInfo.componentTypes[index];
      ComponentInfo componentInfo = new(_entity, index, Activator.CreateInstance(componentType) as IComponent);

      _entity.AddComponent(componentInfo.Index, componentInfo.Component);
    }

    _componentsButton.Selected = -1;
  }
}