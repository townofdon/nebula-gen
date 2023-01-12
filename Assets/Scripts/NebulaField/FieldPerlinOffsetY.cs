using UnityEngine;

public class FieldPerlinOffsetY : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.perlinOffset.y;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.perlinOffset.y = incoming;
        AfterChange();
    }
}
