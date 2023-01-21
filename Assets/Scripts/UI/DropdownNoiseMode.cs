using UnityEngine;

public class DropdownNoiseMode : DropdownBase
{
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
        return (int)nebula2.noiseOptions.noiseMode;
    }

    protected override void OnChange(int enumValue)
    {
        nebula2.noiseOptions.noiseMode = (NebulaGen.FBMNoiseMode)enumValue;
        AfterChange();
    }
}
