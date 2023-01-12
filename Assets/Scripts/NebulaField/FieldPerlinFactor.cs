using UnityEngine;

public class FieldPerlinFactor : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.perlinFactor;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.perlinFactor = incoming;
        AfterChange();
    }
}
