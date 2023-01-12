using UnityEngine;

public class FieldTilingFill : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.tilingFill;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.tilingFill = (int)incoming;
        AfterChange();
    }
}
