using UnityEngine;

public class DropdownNoiseType : DropdownBase
{
    protected override System.Type GetEnumType()
    {
        return typeof(NebulaGen.NoiseType);
    }

    protected override string[] GetExcludedEnumNames()
    {
        return new string[] {
            System.Enum.GetName(typeof(NebulaGen.NoiseType), NebulaGen.NoiseType.Voronoi1),
            System.Enum.GetName(typeof(NebulaGen.NoiseType), NebulaGen.NoiseType.Voronoi2)
        };
    }

    protected override int GetValue()
    {
        return (int)nebula2.noiseOptions.noiseType;
    }

    protected override void OnChange(int enumValue)
    {
        nebula2.noiseOptions.noiseType = (NebulaGen.NoiseType)enumValue;
        AfterChange();
    }
}
