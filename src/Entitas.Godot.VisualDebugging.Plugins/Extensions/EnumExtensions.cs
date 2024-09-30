using System;

namespace Entitas.Godot;

public static class EnumExtensions
{
  public static int IndexOf<TEnum>(this TEnum value)  where TEnum : struct, Enum
  {
    TEnum[] enumValues = Enum.GetValues<TEnum>();
    for (int i=0; i<enumValues.Length; i++)
    {
      TEnum enumValue = enumValues[i];
      if (enumValue.Equals(value))
        return i;
    }

    return -1;
  }
  
  public static TEnum FromIndex<TEnum>(int index)  where TEnum : struct, Enum
  {
    TEnum[] enumValues = Enum.GetValues<TEnum>();
    if (index >= 0 && index <= enumValues.Length)
    {
      return enumValues[index];
    }
    return default;
  }
}