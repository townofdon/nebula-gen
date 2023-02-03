using UnityEngine;
using CyberneticStudios.SOFramework;

public class DropdownNoiseType : DropdownBase
{
    [SerializeField] NoiseTypeVariable noiseType;

    protected override void Init()
    {
        noiseType.ResetVariable();
    }

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
        // return (int)nebula2.noiseOptionsA.noiseType;
        return (int)noiseType.value;
    }

    protected override void OnChange(int enumValue)
    {
        // nebula2.SetNoiseType((NebulaGen.NoiseType)enumValue);
        noiseType.value = (NebulaGen.NoiseType)enumValue;
        AfterChange();
    }
}
