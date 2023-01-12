using UnityEngine;

public class DropdownBorderMode : DropdownBase
{
    protected override System.Type GetEnumType()
    {
        return typeof(NebulaGen.BorderMode);
    }

    protected override int GetValue()
    {
        return (int)nebula2.borderMode;
    }

    protected override void OnChange(int enumValue)
    {
        nebula2.SetBorderMode((NebulaGen.BorderMode)enumValue);
        AfterChange();
    }
}
