using UnityEngine;

public class FieldSwirlAmount : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.swirlAmount;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.swirlAmount = incoming;
        AfterChange();
    }
}
