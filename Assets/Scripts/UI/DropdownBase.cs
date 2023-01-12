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
    TMPro.TMP_Dropdown dropdown;
    protected Nebula2 nebula2;

    // protected void SetOptions(System.Type incomingEnumType)
    // {
    //     dropdown.options.Clear();
    //     dropdown.ClearOptions();
    //     var options = System.Enum.GetNames(incomingEnumType);
    //     for (int i = 0; i < options.Length; i++)
    //     {
    //         dropdown.options.Add(new TMP_Dropdown.OptionData(options[i]));
    //     }
    //     dropdown.value = 0;
    //     if (options.Length > 0) initialOptionText.text = options[0];
    // }

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
            dropdown.options.Add(new TMP_Dropdown.OptionData(options[i]));
        }
        UpdateOptionDisplayValue();
    }

    void OnReinitializeFields()
    {
        UpdateOptionDisplayValue();
    }

    protected abstract System.Type GetEnumType();

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
