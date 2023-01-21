using UnityEngine;

public class FieldPersistence : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.persistence;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.persistence = incoming;
        AfterChange();
    }
}
