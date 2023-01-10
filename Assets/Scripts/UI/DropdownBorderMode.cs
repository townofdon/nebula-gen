using UnityEngine;

public class DropdownBorderMode : DropdownBase
{
    void Start()
    {
        SetOptions(typeof(NebulaGen.BorderMode));
    }

    void OnEnable()
    {
        dropdown.onValueChanged.AddListener(OnChange);
    }

    void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(OnChange);
    }

    void OnChange(int enumValue)
    {
        nebula2.SetBorderMode((NebulaGen.BorderMode)enumValue);
        AfterChange();
    }
}
