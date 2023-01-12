using UnityEngine;

public class DropdownNoiseMode : DropdownBase
{
    protected override System.Type GetEnumType()
    {
        return typeof(NebulaGen.FBMNoiseMode);
    }

    protected override int GetValue()
    {
        return (int)nebula2.noiseLayerA.noiseMode;
    }

    protected override void OnChange(int enumValue)
    {
        nebula2.noiseLayerA.noiseMode = (NebulaGen.FBMNoiseMode)enumValue;
        AfterChange();
    }
}
