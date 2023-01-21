using UnityEngine;

public class FieldPerlinOffsetY : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.perlinOffset.y;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.perlinOffset.y = incoming;
        AfterChange();
    }
}
