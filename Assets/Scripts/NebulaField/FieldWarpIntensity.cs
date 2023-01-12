using UnityEngine;

public class FieldWarpIntensity : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.warpIntensity;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.warpIntensity = incoming;
        AfterChange();
    }
}
