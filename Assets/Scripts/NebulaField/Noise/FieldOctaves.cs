using UnityEngine;

public class FieldOctaves : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.octaves;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.octaves = (int)incoming;
        AfterChange();
    }
}
