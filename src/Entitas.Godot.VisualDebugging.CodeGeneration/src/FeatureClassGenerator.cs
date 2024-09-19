using Jenny;

namespace Entitas.Godot
{
    public class FeatureClassGenerator : ICodeGenerator
    {
        public string Name => "Feature Class";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string FEATURE_TEMPLATE =
            @"#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && GODOT && DEBUG)

public class Feature : Entitas.Godot.DebugSystems {

    public Feature(string name) : base(name) {
    }

    public Feature() {
    }
}

#else

public class Feature : Entitas.Systems {

    public Feature(string name) {
    }

    public Feature() {
    }
}

#endif
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => new[]
        {
            new CodeGenFile(
                "Feature.cs",
                FEATURE_TEMPLATE,
                GetType().FullName)
        };
    }
}