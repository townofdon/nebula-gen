using UnityEngine;

public class FieldPerlinOffsetX : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.perlinOffset.x;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.perlinOffset.x = incoming;
        AfterChange();
    }
}
