using UnityEngine;

public class FieldMaskSoftness : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.maskSoftness;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.maskSoftness = incoming;
        nebula2.mixMask = GetMixValueFromSoftness(incoming);
        AfterChange();
    }

    public static float GetMixValueFromSoftness(float softness)
    {
        return Mathf.Lerp(1f, 0.85f, softness);
    }
}
