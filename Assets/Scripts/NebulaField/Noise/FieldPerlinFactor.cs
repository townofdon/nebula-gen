using UnityEngine;

public class FieldPerlinFactor : NoiseLayerFieldBase
{
    // note - make sure the slider min/max matches these values
    private const float MIN = 0.01f;
    private const float MAX = 5f;

    protected override float GetInitialValue()
    {
        float value = 0f;
        if (noiseLayer == NoiseLayer.A)
        {
            value = nebula2.noiseOptionsA.perlinFactor;
        }
        else
        {
            value = nebula2.noiseOptionsB.perlinFactor;
        }
        return Mathf.Lerp(MIN, MAX, Easing.InQuadInverse(Mathf.InverseLerp(MIN, MAX, value)));
    }

    protected override void OnValueChanged(float incoming)
    {
        float value = Mathf.Lerp(MIN, MAX, Easing.InQuad(Mathf.InverseLerp(MIN, MAX, incoming)));
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.perlinFactor = value;
        }
        else
        {
            nebula2.noiseOptionsB.perlinFactor = value;
        }
        AfterChange();
    }
}
