using UnityEngine;

public class FieldEdgeVarianceEffect : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.edgeVarianceEffect;
    }

    protected override void OnValueChanged(float incoming)
    {
        // lerp from 0-100 for values 0-100, then 100-200 for values 100-1000
        float effectValue = Mathf.Lerp(
            0,
            Mathf.Lerp(100f, 200f, (incoming - 100f) / 900f),
            incoming / 100f
        );
        float strength = Mathf.Lerp(100f, 150f, incoming * 0.01f);
        float perlinOffsetX = 100f + incoming * 0.01f;
        float perlinOffsetY = 100f + incoming * -0.005f;
        float perlinFactor = Mathf.Lerp(.2f, .5f, incoming * 0.001f);
        float persistence = Mathf.Lerp(0.5f, 1.5f, incoming * 0.001f);

        nebula2.edgeVarianceEffect = effectValue;
        nebula2.edgeVarianceStrength = strength;
        nebula2.falloffOptions.perlinOffset.x = perlinOffsetX;
        nebula2.falloffOptions.perlinOffset.y = perlinOffsetY;
        nebula2.falloffOptions.perlinFactor = perlinFactor;
        nebula2.falloffOptions.persistence = persistence;
        AfterChange();
    }
}
