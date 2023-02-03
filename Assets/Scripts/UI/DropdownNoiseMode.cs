using UnityEngine;
using CyberneticStudios.SOFramework;

public class DropdownNoiseMode : DropdownBase
{
    [SerializeField] NoiseModeVariable noiseMode;

    protected override void Init()
    {
        noiseMode.ResetVariable();
    }

    protected override System.Type GetEnumType()
    {
        return typeof(NebulaGen.FBMNoiseMode);
    }

    protected override string[] GetExcludedEnumNames()
    {
        return new string[0];
    }

    protected override int GetValue()
    {
        // return (int)nebula2.noiseOptionsA.noiseMode;
        return (int)noiseMode.value;
    }

    protected override void OnChange(int enumValue)
    {
        // nebula2.noiseOptionsA.noiseMode = (NebulaGen.FBMNoiseMode)enumValue;
        noiseMode.value = (NebulaGen.FBMNoiseMode)enumValue;
        AfterChange();
    }
}
