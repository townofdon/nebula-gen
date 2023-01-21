using UnityEngine;

public class FieldLacunarity : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.lacunarity;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.lacunarity = incoming;
        AfterChange();
    }
}
