using UnityEngine;

public class FieldEdgeCut : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.edgeCutStrength;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.edgeCutStrength = incoming;
        AfterChange();
    }
}
