using UnityEngine;

public class FieldEnableMask : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.mixMask;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.mixMask = incoming;
        AfterChange();
    }
}