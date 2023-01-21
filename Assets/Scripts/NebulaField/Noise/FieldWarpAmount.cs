using UnityEngine;

public class FieldWarpAmount : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.warpAmount;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.warpAmount = incoming;
        AfterChange();
    }
}
