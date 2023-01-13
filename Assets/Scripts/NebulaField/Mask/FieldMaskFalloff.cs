using UnityEngine;

public class FieldMaskFalloff : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.maskFalloff;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.maskFalloff = incoming;
        AfterChange();
    }
}
