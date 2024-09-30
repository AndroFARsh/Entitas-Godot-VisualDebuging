using System.Linq;
using Jenny;
using DesperateDevs.Extensions;
using Entitas.CodeGeneration.Plugins;

namespace Entitas.Godot
{
  public class ContextObserverGenerator : ICodeGenerator
  {
    public string Name => "Context Observer";
    public int Order => 0;
    public bool RunInDryMode => true;

    const string CONTEXTS_TEMPLATE =
      @"public partial class Contexts {

#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && GODOT && DEBUG)
  [Entitas.CodeGeneration.Attributes.PostConstructor]
  public void InitializeContextObservers() 
  {
    try {
${contextObservers}
    } catch(System.Exception e) {
      Godot.GD.PrintErr(e);
    }
  }

  public void CreateContextObserver(Entitas.IContext context)
  { 
    Entitas.Godot.EntitasRoot.Register(context);
  }

#endif
}
";

    const string CONTEXT_OBSERVER_TEMPLATE = @"            CreateContextObserver(${contextName});";

    public CodeGenFile[] Generate(CodeGeneratorData[] data)
    {
      var contextNames = data
        .OfType<ContextData>()
        .Select(d => d.GetContextName())
        .OrderBy(contextName => contextName)
        .ToArray();

      return new[]
      {
        new CodeGenFile(
          "Contexts.cs",
          generateContextsClass(contextNames),
          GetType().FullName)
      };
    }

    string generateContextsClass(string[] contextNames)
    {
      string contextObservers = string.Join("\n", contextNames
        .Select(contextName => CONTEXT_OBSERVER_TEMPLATE
          .Replace("${contextName}", contextName.ToLowerFirst())));

      return CONTEXTS_TEMPLATE
        .Replace("${contextObservers}", contextObservers);
    }
  }
}