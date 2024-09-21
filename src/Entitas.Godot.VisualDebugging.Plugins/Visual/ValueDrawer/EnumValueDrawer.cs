using System;
using Godot;

namespace Entitas.Godot;

public partial class EnumValueDrawer : BaseValueDrawer
{
  private OptionButton _optionButton;

  private string[] _enumNames;
  private Array _enumValues;

  protected override void InitializeDrawer()
  {
    _optionButton ??= CreateRow();
    _optionButton.ItemSelected += OnItemSelected;

    object value = ComponentInfo.GetFieldValue(FieldName);
    _enumValues = Enum.GetValues(value.GetType());
    _enumNames = Enum.GetNames(value.GetType());

    foreach (string enumName in _enumNames)
      _optionButton.AddItem(enumName);
  }

  private OptionButton CreateRow()
  {
    OptionButton optionButton = new();
    optionButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    GridContainer.AddChild(optionButton);
    return optionButton;
  }

  protected override void CleanUpDrawer()
  {
    _optionButton.Clear();
    _optionButton.ItemSelected -= OnItemSelected;
  }

  public override void UpdateValue(object value)
  {
    object id = ToUnderlyingType(value);
    for (int i = 0; i < _enumValues.Length; i++)
    {
      object v = ToUnderlyingType(_enumValues.GetValue(i));
      if (IsSame(id, v))
      {
        _optionButton.Selected = i;
        return;
      }
    }

    _optionButton.Selected = -1;
  }

  private void OnItemSelected(long index) => ComponentInfo.SetFieldValue(FieldName, _enumValues.GetValue(index));

  private static object ToUnderlyingType(object value)
  {
    Type type = value.GetType();
    Type underType = Enum.GetUnderlyingType(type);
    return Convert.ChangeType(value, underType);
  }

  private static bool IsSame(object v1, object v2)
  {
    TypeCode typeCodeV1 = Convert.GetTypeCode(v1);
    TypeCode typeCodeV2 = Convert.GetTypeCode(v2);
    
    if (typeCodeV1 != typeCodeV2) return false;

    return typeCodeV1 switch
    {
      TypeCode.Boolean => (bool)v1 == (bool)v2,
      TypeCode.Char => (char)v1 == (char)v2,
      TypeCode.SByte => (sbyte)v1 == (sbyte)v2,
      TypeCode.Byte => (byte)v1 == (byte)v2,
      TypeCode.Int16 => (short)v1 == (short)v2,
      TypeCode.UInt16 => (ushort)v1 == (ushort)v2,
      TypeCode.Int32 => (int)v1 == (int)v2,
      TypeCode.UInt32 => (uint)v1 == (uint)v2,
      TypeCode.Int64 => (long)v1 == (long)v2,
      TypeCode.UInt64 => (ulong)v1 == (ulong)v2,
      TypeCode.Single => Math.Abs((float)v1 - (float)v2) < float.Epsilon,
      TypeCode.Double => Math.Abs((double)v1 - (double)v2) < double.Epsilon,
      _ => false
    };
  }
}