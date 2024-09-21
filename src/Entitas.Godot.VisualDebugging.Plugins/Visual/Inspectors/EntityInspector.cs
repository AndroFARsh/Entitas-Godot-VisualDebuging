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
  private EntityObserverNode _entityObserverNode;
  
  private readonly Dictionary<ComponentInfo, ComponentDrawer> _drawers = new();

  public override void Initialize(Node node)
  {
    if (node == _entityObserverNode) return;
    _entityObserverNode = (EntityObserverNode)node;
    _contextInfo = _entityObserverNode.Context.contextInfo;
    
    InitializeTree();

    _componentsButton.ItemSelected += OnComponentAddAction;
    _entityObserverNode.ComponentInfoAction += OnComponentInfoAction;
    _destroyEntityButton.Pressed += OnDestroyEntityPressed;
    _cloneEntityButton.Pressed += OnCloneEntityPressed;
    _searchTextEdit.TextChanged += OnSearchTextChanged;
    _clearSearchButton.Pressed += OnClearSearchPressed;

    FillComponentDropDown();

    foreach (ComponentInfo componentInfo in _entityObserverNode.Components)
    {
      ComponentDrawer componentDrawer = CreateComponentDrawer(_entityObserverNode, componentInfo);
      _componentsContainer.AddChild(componentDrawer);
      _drawers.Add(componentInfo, componentDrawer);
    }

    Visible = true;
  }

  private void InitializeTree()
  {
    if (GetChildCount() > 0) return;
    
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

    HBoxContainer addComponentContent = new HBoxContainer();
    addComponentContent.AddThemeConstantOverride("separation", Consts.Margin);
    content.AddChild(addComponentContent);

    Label addComponentTitle = new Label();
    addComponentTitle.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    addComponentTitle.Text = "Add Component";
    addComponentContent.AddChild(addComponentTitle);
    
    _componentsButton = new OptionButton();
    _componentsButton.AddThemeFontSizeOverride("font_size", Consts.FontTitleSize);
    _componentsButton.CustomMinimumSize = new Vector2(0, 50);
    _componentsButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    _componentsButton.FitToLongestItem = false;
    addComponentContent.AddChild(_componentsButton);
    
    HBoxContainer searchContent = new HBoxContainer();
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
    content.AddChild(_componentsContainer);
  }

  private void FillComponentDropDown()
  {
    _componentsButton.Clear();
    
    IEntity entity = _entityObserverNode?.Entity;
    if (entity == null) return;

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

  private ComponentDrawer CreateComponentDrawer(EntityObserverNode entityObserverNode, ComponentInfo componentInfo)
  {
    if (!EntitasRoot.Pool.Request(typeof(ComponentInfo), out ComponentDrawer drawer))
      drawer = new ComponentDrawer();

    drawer.Initialize(entityObserverNode, componentInfo);
    return drawer;
  }

  public override void CleanUp()
  {
    if (_entityObserverNode == null) return;

    _componentsButton.Clear();

    foreach ((ComponentInfo _, ComponentDrawer drawer) in _drawers)
    {
      drawer.CleanUp();
      _componentsContainer.RemoveChild(drawer);

      EntitasRoot.Pool.Retain(typeof(ComponentInfo), drawer);
    }

    _drawers.Clear();

    _componentsButton.ItemSelected -= OnComponentAddAction;
    _entityObserverNode.ComponentInfoAction -= OnComponentInfoAction;
    _destroyEntityButton.Pressed -= OnDestroyEntityPressed;
    _cloneEntityButton.Pressed -= OnCloneEntityPressed;
    _searchTextEdit.TextChanged -= OnSearchTextChanged;
    _clearSearchButton.Pressed -= OnClearSearchPressed;

    _entityObserverNode = null;
    _contextInfo = null;
    Visible = false;
  }

  private void OnClearSearchPressed()
  {
    _searchTextEdit.Text = "";
    OnSearchTextChanged();
  }

  private void OnComponentInfoAction(ComponentActionType action, ComponentInfo componentInfo)
  {
    switch (action)
    {
      case ComponentActionType.Added:
      {
        ComponentDrawer componentDrawer = CreateComponentDrawer(_entityObserverNode, componentInfo);
        _componentsContainer.AddChild(componentDrawer);
        _drawers.Add(componentInfo, componentDrawer);

        FillComponentDropDown();
        break;
      }
      case ComponentActionType.Removed:
      {
        if (_drawers.TryGetValue(componentInfo, out ComponentDrawer drawer))
        {
          drawer.CleanUp();
          EntitasRoot.Pool.Retain(typeof(ComponentInfo), drawer);
          _drawers.Remove(componentInfo);
          _componentsContainer.RemoveChild(drawer);

          FillComponentDropDown();
        }

        break;
      }
    }
  }

  private void OnDestroyEntityPressed() => _entityObserverNode.Entity.Destroy();

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
    IEntity newEntity = _entityObserverNode.Context.CreateEntity();
    foreach (ComponentInfo componentInfo in _entityObserverNode.Components)
    {
      ComponentInfo newComponentInfo = new(newEntity, componentInfo.Index,
        Activator.CreateInstance(componentInfo.Type) as IComponent);
      foreach (string fieldName in componentInfo.FieldNames)
        newComponentInfo.SetFieldValue(fieldName, componentInfo.GetFieldValue(fieldName));

      newEntity.AddComponent(newComponentInfo.Index, newComponentInfo.Component);
    }
  }

  private void OnComponentAddAction(long i)
  {
    int index = _componentsButton.GetItemId((int)i);
    IEntity entity = _entityObserverNode.Entity;

    if (!entity.HasComponent(index))
    {
      Type componentType = _contextInfo.componentTypes[index];
      ComponentInfo componentInfo = new(entity, index, Activator.CreateInstance(componentType) as IComponent);

      entity.AddComponent(componentInfo.Index, componentInfo.Component);
    }

    _componentsButton.Selected = -1;
  }
}