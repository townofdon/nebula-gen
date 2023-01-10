using UnityEngine;
using TMPro;
using NebulaGen;

[RequireComponent(typeof(TMP_Dropdown))]
public abstract class DropdownBase : MonoBehaviour
{
    NebulaGen.NoiseType noiseType;

    System.Type enumType;
    protected TMPro.TMP_Dropdown dropdown;
    protected Nebula2 nebula2;

    protected void SetOptions(System.Type incomingEnumType)
    {
        enumType = incomingEnumType;
        dropdown.options.Clear();
        foreach (var option in System.Enum.GetNames(incomingEnumType))
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(option));
        }
    }

    protected void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        nebula2 = FindObjectOfType<Nebula2>();
    }

    protected void AfterChange()
    {
        nebula2.GenerateNoise();
    }
}
