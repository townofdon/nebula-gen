using UnityEngine;

public class DropdownNoiseType : DropdownBase
{
    protected override System.Type GetEnumType()
    {
        return typeof(NebulaGen.NoiseType);
    }

    protected override int GetValue()
    {
        return (int)nebula2.noiseLayerA.noiseType;
    }

    protected override void OnChange(int enumValue)
    {
        nebula2.noiseLayerA.noiseType = (NebulaGen.NoiseType)enumValue;
        AfterChange();
    }
}
