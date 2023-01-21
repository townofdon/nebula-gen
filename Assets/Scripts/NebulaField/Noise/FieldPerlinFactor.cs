using UnityEngine;

public class FieldPerlinFactor : FieldBase
{
    // note - make sure the slider min/max matches these values
    private const float MIN = 0.01f;
    private const float MAX = 5f;

    protected override float GetInitialValue()
    {
        return Mathf.Lerp(MIN, MAX, Easing.InQuadInverse(Mathf.InverseLerp(MIN, MAX, nebula2.noiseOptions.perlinFactor)));
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.perlinFactor = Mathf.Lerp(MIN, MAX, Easing.InQuad(Mathf.InverseLerp(MIN, MAX, incoming)));
        AfterChange();
    }
}
