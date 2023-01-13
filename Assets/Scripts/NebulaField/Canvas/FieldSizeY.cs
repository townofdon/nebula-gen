using UnityEngine;

public class FieldSizeY : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.sizeY;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.sizeY = (int)Mathf.Max(incoming, 16);
        AfterChange();
        nebula2.DrawOutput();
    }
}
