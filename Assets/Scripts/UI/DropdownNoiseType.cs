using UnityEngine;

public class DropdownNoiseType : DropdownBase
{
    void Start()
    {
        SetOptions(typeof(NebulaGen.NoiseType));
    }

    void OnEnable()
    {
        dropdown.onValueChanged.AddListener(OnChange);
    }

    void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(OnChange);
    }

    void OnChange(int enumValue)
    {
        Debug.Log(System.Enum.GetName(typeof(NebulaGen.NoiseType), enumValue));
        nebula2.noiseLayerA.noiseType = (NebulaGen.NoiseType)enumValue;
        AfterChange();
    }
}
