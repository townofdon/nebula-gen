using UnityEngine;

public class FieldPerlinOffsetX : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.perlinOffset.x;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.perlinOffset.x = incoming;
        AfterChange();
    }

    void LateUpdate()
    {
        UpdateUI();
    }
}
