using UnityEngine;

public class FieldSizeX : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.sizeX;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.sizeX = (int)Mathf.Max(incoming, 16);
        AfterChange();
        nebula2.DrawOutput();
    }
}
