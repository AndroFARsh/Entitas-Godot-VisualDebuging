//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class MetaMatcher {

    static Entitas.IMatcher<MetaEntity> _matcherFlag;

    public static Entitas.IMatcher<MetaEntity> Flag {
        get {
            if (_matcherFlag == null) {
                var matcher = (Entitas.Matcher<MetaEntity>)Entitas.Matcher<MetaEntity>.AllOf(MetaComponentsLookup.Flag);
                matcher.componentNames = MetaComponentsLookup.componentNames;
                _matcherFlag = matcher;
            }

            return _matcherFlag;
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by KSyndicate.CustomGenerators.Plugins.SingleValueComponentEntityApiInterfaceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class MetaEntity : IFlagEntity { }

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class MetaEntity {

    static readonly Entitas.Godot.FlagComponent flagComponent = new Entitas.Godot.FlagComponent();

    public bool isFlag {
        get { return HasComponent(MetaComponentsLookup.Flag); }
        set {
            if (value != isFlag) {
                var index = MetaComponentsLookup.Flag;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : flagComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
