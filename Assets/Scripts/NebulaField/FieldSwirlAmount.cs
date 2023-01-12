using UnityEngine;

public class FieldSwirlAmount : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.swirlAmount;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.swirlAmount = incoming;
        AfterChange();
    }
}
