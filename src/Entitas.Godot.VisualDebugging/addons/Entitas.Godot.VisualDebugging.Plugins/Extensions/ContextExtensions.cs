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
    MethodInfo[] methods = context.GetType()
      .GetMethods(BindingFlags.Public | BindingFlags.Instance);
    MethodInfo method = methods.FirstOrDefault(m => m.Name == "GetEntities");
      //.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
      //.FirstOrDefault(m => m.Name == "_entities");
    return method?.Invoke(context, null) as IEntity[];
  }
  
  // public static IEnumerable<IGroup<IEntity>> Groups(this IContext context)
  // {
  //   FieldInfo field = context.GetType()
  //     .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
  //     .FirstOrDefault(m => m.Name == "_groups");
  //   return (field?.GetValue(context) as Dictionary<IMatcher<IEntity>, IGroup<IEntity>>)?.Values;
  // }
}