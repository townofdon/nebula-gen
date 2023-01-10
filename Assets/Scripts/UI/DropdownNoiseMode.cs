using UnityEngine;

public class DropdownNoiseMode : DropdownBase
{
    void Start()
    {
        SetOptions(typeof(NebulaGen.FBMNoiseMode));
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
        Debug.Log(System.Enum.GetName(typeof(NebulaGen.FBMNoiseMode), enumValue));
        nebula2.noiseLayerA.noiseMode = (NebulaGen.FBMNoiseMode)enumValue;
        AfterChange();
    }
}
