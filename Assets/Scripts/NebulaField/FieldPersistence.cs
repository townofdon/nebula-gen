using UnityEngine;

public class FieldPersistence : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.persistence;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.persistence = incoming;
        AfterChange();
    }
}
