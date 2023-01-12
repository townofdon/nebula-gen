using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using NebulaGen;

[RequireComponent(typeof(TMP_Dropdown))]
public abstract class DropdownBase : MonoBehaviour
{
    DropdownInitialOption initialOption;
    TextMeshProUGUI initialOptionText;

    NebulaGen.NoiseType noiseType;

    System.Type enumType;
    protected TMPro.TMP_Dropdown dropdown;
    protected Nebula2 nebula2;

    protected void SetOptions(System.Type incomingEnumType)
    {
        enumType = incomingEnumType;
        dropdown.options.Clear();
        dropdown.ClearOptions();
        var options = System.Enum.GetNames(incomingEnumType);
        for (int i = 0; i < options.Length; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(options[i]));
        }
        dropdown.value = 0;
        if (options.Length > 0) initialOptionText.text = options[0];
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

    protected void AfterChange()
    {
        nebula2.GenerateNoise();
    }
}
