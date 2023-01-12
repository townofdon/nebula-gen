using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using NebulaGen;
using System;

[RequireComponent(typeof(TMP_Dropdown))]
public abstract class DropdownBase : MonoBehaviour
{
    DropdownInitialOption initialOption;
    TextMeshProUGUI initialOptionText;

    string[] options;
    string[] excluded;
    TMPro.TMP_Dropdown dropdown;
    protected Nebula2 nebula2;

    protected void OnEnable()
    {
        dropdown.onValueChanged.AddListener(OnChange);
        FieldEvent.OnReinitializeFields += OnReinitializeFields;
    }

    protected void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(OnChange);
        FieldEvent.OnReinitializeFields -= OnReinitializeFields;
    }

    protected void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        nebula2 = FindObjectOfType<Nebula2>();
        initialOption = GetComponentInChildren<DropdownInitialOption>();
        Assert.IsNotNull(initialOption);
        initialOptionText = initialOption.GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(initialOptionText);
    }

    protected void Start()
    {
        System.Type enumType = GetEnumType();
        dropdown.options.Clear();
        dropdown.ClearOptions();
        options = System.Enum.GetNames(enumType);
        for (int i = 0; i < options.Length; i++)
        {
            if (IsOptionExcluded(options[i])) continue;
            dropdown.options.Add(new TMP_Dropdown.OptionData(options[i]));
        }
        UpdateOptionDisplayValue();
    }

    bool IsOptionExcluded(string option)
    {
        if (excluded == null) excluded = GetExcludedEnumNames();
        for (int i = 0; i < excluded.Length; i++)
        {
            if (excluded[i] == option) return true;
        }
        return false;
    }

    void OnReinitializeFields()
    {
        UpdateOptionDisplayValue();
    }

    protected abstract System.Type GetEnumType();

    protected abstract string[] GetExcludedEnumNames();

    protected abstract int GetValue();

    protected abstract void OnChange(int enumValue);

    protected void AfterChange()
    {
        UpdateOptionDisplayValue();
        nebula2.GenerateNoise();
    }

    void UpdateOptionDisplayValue()
    {
        dropdown.value = GetValue();
        if (options.Length > GetValue()) initialOptionText.text = options[GetValue()];
    }
}
