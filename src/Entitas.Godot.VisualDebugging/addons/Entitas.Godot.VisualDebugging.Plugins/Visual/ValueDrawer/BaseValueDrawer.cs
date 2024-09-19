using Godot;

namespace Entitas.Godot;

public abstract partial class BaseValueDrawer : GridContainer
{
  private string _fieldName;
  private ComponentInfo _componentInfo;
  
  public string FieldName => _fieldName;

  public ComponentInfo ComponentInfo => _componentInfo;

  public GridContainer GridContainer => this;
  
  public void Initialize(ComponentInfo componentInfo, string fieldName)
  {
    _componentInfo = componentInfo;
    _fieldName = fieldName;
    
    SizeFlagsHorizontal = SizeFlags.ExpandFill;
    InitializeDrawer();
    UpdateValue(_componentInfo.GetFieldValue(_fieldName));
  }

  public void CleanUp()
  {
    CleanUpDrawer();
    
    _componentInfo = null;
    _fieldName = null;
  }

  protected abstract void CleanUpDrawer();
  
  protected abstract void InitializeDrawer();

  public abstract void UpdateValue(object value);
}