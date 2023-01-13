using UnityEngine;

public class FieldMaskSelectPoint : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.maskSelectPoint;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.maskSelectPoint = incoming;
        AfterChange();
    }
}
