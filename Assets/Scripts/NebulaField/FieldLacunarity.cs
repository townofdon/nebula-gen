using UnityEngine;

public class FieldLacunarity : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.lacunarity;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.lacunarity = incoming;
        AfterChange();
    }
}
