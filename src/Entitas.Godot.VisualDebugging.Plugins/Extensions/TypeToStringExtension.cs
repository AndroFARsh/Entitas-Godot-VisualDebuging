using System;

namespace Extensions;

public static class TypeToStringExtension
{
  public static string TypeName(this Type type)
  {
    string fullTypeName = type.ToString();
    int startIndex = fullTypeName.LastIndexOf(".", StringComparison.Ordinal) + 1;
    return fullTypeName.Substring(startIndex, fullTypeName.Length - startIndex);
  }

  public static string RemoveSuffix(this string fullName, string suffix)
  {
    int last = fullName.LastIndexOf(suffix, StringComparison.Ordinal) + 1;
    return last > 0 
      ? fullName.Substring(0, last)
      : fullName;
  } 
}