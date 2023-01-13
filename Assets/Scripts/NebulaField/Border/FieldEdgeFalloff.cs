using UnityEngine;

public class FieldEdgeFalloff : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.edgeFalloff;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.edgeFalloff = incoming;
        AfterChange();
    }
}
