using UnityEngine;

public class FieldOctaves : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.octaves;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.octaves = (int)incoming;
        AfterChange();
    }
}
