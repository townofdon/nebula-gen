using UnityEngine;

public class FieldEdgeDistance : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.edgeDistance;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.edgeDistance = incoming;
        AfterChange();
    }
}
