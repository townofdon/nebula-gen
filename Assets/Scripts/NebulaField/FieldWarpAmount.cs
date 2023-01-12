using UnityEngine;

public class FieldWarpAmount : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.warpAmount;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.warpAmount = incoming;
        AfterChange();
    }
}
