using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CyberneticStudios.SOFramework;

// The SO Framework allows us to decouple concerns between the state
// being updated and things that subscribe to the state update event.

[RequireComponent(typeof(Selectable))]
public class FieldValue : MonoBehaviour
{
    [SerializeField] FloatVariable variable;
    [SerializeField] bool initializeOnAwake = true;

    NebulaGen.Nebula2 nebula2;

    Selectable selectable;
    TMP_InputField input;
    Slider slider;
    Toggle toggle;

    void OnEnable()
    {
        input?.onValueChanged.AddListener(OnValueChanged);
        slider?.onValueChanged.AddListener(OnValueChanged);
        toggle?.onValueChanged.AddListener(OnValueChanged);
    }

    void OnDisable()
    {
        input?.onValueChanged.RemoveListener(OnValueChanged);
        slider?.onValueChanged.RemoveListener(OnValueChanged);
        toggle?.onValueChanged.RemoveListener(OnValueChanged);
    }

    void Awake()
    {
        nebula2 = FindObjectOfType<NebulaGen.Nebula2>();
        selectable = GetComponent<Selectable>();
        // the `as` cast will not throw an exception, but just return `null`
        input = selectable as TMP_InputField;
        slider = selectable as Slider;
        toggle = selectable as Toggle;

        if (initializeOnAwake) variable.ResetVariable();
        UpdateUI();
    }

    void UpdateUI()
    {
        float value = variable.value;
        if (input) input.text = value.ToString();
        if (slider) slider.value = value;
        if (toggle) toggle.isOn = value > float.Epsilon;
    }

    void OnValueChanged(string incoming)
    {
        if (float.TryParse(incoming, out float result))
        {
            OnValueChanged(result);
        }
    }

    void OnValueChanged(bool incoming)
    {
        OnValueChanged(incoming ? 1 : 0);
    }

    void OnValueChanged(float incoming)
    {
        variable.value = incoming;
        UpdateUI();
        nebula2.GenerateNoise();
    }
}
