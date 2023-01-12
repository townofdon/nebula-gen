using UnityEngine;

public class FieldSwirlIntensity : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.swirlIntensity;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.swirlIntensity = incoming;
        AfterChange();
    }
}
