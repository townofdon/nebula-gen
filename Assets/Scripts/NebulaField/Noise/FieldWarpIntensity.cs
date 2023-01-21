using UnityEngine;

public class FieldWarpIntensity : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.warpIntensity;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.warpIntensity = incoming;
        AfterChange();
    }
}
