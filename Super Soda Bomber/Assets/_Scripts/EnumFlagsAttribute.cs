using UnityEngine;

/*
EnumFlagsAttribute
    Supports multi-selection for enumerators.
    Mainly used for mix-and-match player abilities.
*/
public class EnumFlagsAttribute : PropertyAttribute {
    public string enumName;

    public EnumFlagsAttribute() { }

    public EnumFlagsAttribute(string name) {
        enumName = name;
    }
}
