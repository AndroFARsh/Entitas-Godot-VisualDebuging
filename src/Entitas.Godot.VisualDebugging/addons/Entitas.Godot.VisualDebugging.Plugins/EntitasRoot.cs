using Godot;
using Microsoft.VisualBasic;

namespace Entitas.Godot;

[GlobalClass]
public partial class EntitasRoot : CanvasLayer
{
  private const int EntitasInspectorId = 122412;
    
  private static EntitasRoot _global;

  private readonly NodePool _pool = new();
  private Node _entities;
  private Node _systems;
  private Window _window;
  private MenuButton _debugMenuButton;
  
  private PackedScene _entityInspectorWindow =
    ResourceLoader.Load<PackedScene>(VdConst.WindowResourcePath);

  public static EntitasRoot Global => _global ??= CreateNewInstance();
  
  public static NodePool Pool => Global._pool;
  
  public static Node Systems => Global._systems;
  
  public static Node Entities => Global._entities;
  
  private static EntitasRoot CreateNewInstance()
  {
    EntitasRoot newInstance = new();
    Node root = ((SceneTree)Engine.GetMainLoop()).CurrentScene;
    root.AddChild(newInstance);
    return newInstance;
  }

  public override void _EnterTree()
  {
    if (_global == null)
    {
      Initialize();
      _global = this;
      return;
    }
    QueueFree();
  }
  
  private void OnInspectorWindowClosed()
  {
    _window.CloseRequested -= OnInspectorWindowClosed;
    _window = null;
  }

  public override void _Notification(int what)
  {
    if (what == NotificationPredelete)
    {
      if (_global == this)
      {
        _pool.Dispose();
        _entityInspectorWindow = null;
        _entities = null;
        _systems = null;
        _window = null;
        _debugMenuButton = null;
        _global = null;
        QueueFree();
      }
    }
  }
  
  private void Initialize()
  {
    MenuBar menuBar = new();
    menuBar.PreferGlobalMenu = true;
    
    AddChild(menuBar);
    _debugMenuButton = new MenuButton();
    _debugMenuButton.Text = "Debug Tools";
    _debugMenuButton.GetPopup().AddItem("Entitas Inspector", EntitasInspectorId);
    _debugMenuButton.GetPopup().IdPressed += OnMenuItemPressed;
    menuBar.AddChild(_debugMenuButton);
    
    Name = "[Entitas]";

    _systems = new Node();
    _systems.Name = "[Systems]";
    AddChild(_systems);

    _entities = new Node();
    _entities.Name = "[Entities]";
    AddChild(_entities);
  }

  private void OnMenuItemPressed(long id)
  {
    _debugMenuButton.GetPopup().PreferNativeMenu = true;
    if (id == EntitasInspectorId && _window == null)
    {
      GetViewport().SetEmbeddingSubwindows(false);
      _window = _entityInspectorWindow.Instantiate<Window>();
      _window.CloseRequested += OnInspectorWindowClosed;
      AddChild(_window);
      _window.Title = "Entitas Inspector";
      _window.SetPosition(GetTree().Root.Position);
    }
  }
}