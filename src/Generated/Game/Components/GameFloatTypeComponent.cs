//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherFloatType;

    public static Entitas.IMatcher<GameEntity> FloatType {
        get {
            if (_matcherFloatType == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.FloatType);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherFloatType = matcher;
            }

            return _matcherFloatType;
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Entitas.Godot.FloatTypeComponent floatType { get { return (Entitas.Godot.FloatTypeComponent)GetComponent(GameComponentsLookup.FloatType); } }
    public float FloatType { get { return floatType.Value; } }
    public bool hasFloatType { get { return HasComponent(GameComponentsLookup.FloatType); } }

    public GameEntity AddFloatType(float newValue) {
        var index = GameComponentsLookup.FloatType;
        var component = (Entitas.Godot.FloatTypeComponent)CreateComponent(index, typeof(Entitas.Godot.FloatTypeComponent));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceFloatType(float newValue) {
        var index = GameComponentsLookup.FloatType;
        var component = (Entitas.Godot.FloatTypeComponent)CreateComponent(index, typeof(Entitas.Godot.FloatTypeComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveFloatType() {
        RemoveComponent(GameComponentsLookup.FloatType);
        return this;
    }
}
