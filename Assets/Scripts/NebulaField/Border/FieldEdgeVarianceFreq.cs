using UnityEngine;

public class FieldEdgeVarianceFreq : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.edgeVarianceEffect;
    }

    protected override void OnValueChanged(float incoming)
    {
        float perlinFactor = Mathf.Lerp(.2f, .5f, incoming * 0.001f);
        float persistence = Mathf.Lerp(0.5f, 1.5f, incoming * 0.001f);

        nebula2.falloffOptions.perlinFactor = perlinFactor;
        nebula2.falloffOptions.persistence = persistence;
        AfterChange();
    }
}
