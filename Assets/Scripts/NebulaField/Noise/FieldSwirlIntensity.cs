using UnityEngine;

public class FieldSwirlIntensity : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.swirlIntensity;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.swirlIntensity = incoming;
        AfterChange();
    }
}
