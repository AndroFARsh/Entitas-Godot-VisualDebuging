using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Entitas.Godot;

public static class ContextExtensions
{
  public static IEntity CreateEntity(this IContext context)
  {
    MethodInfo method = context.GetType()
      .GetMethods(BindingFlags.Public | BindingFlags.Instance)
      .FirstOrDefault(m => m.Name == "CreateEntity");
    return method?.Invoke(context, null) as IEntity;
  }
  
  public static IEnumerable<IEntity> Entities(this IContext context)
  {
    MethodInfo method = context.GetType().
      GetMethods(BindingFlags.Public | BindingFlags.Instance)
      .FirstOrDefault(m => m.Name == "GetEntities");
    return method?.Invoke(context, null) as IEntity[];
  }
}