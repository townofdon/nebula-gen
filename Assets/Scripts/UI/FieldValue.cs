using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using TMPro;
using CyberneticStudios.SOFramework;

// The SO Framework allows us to decouple concerns between the state
// being updated and things that subscribe to the state update event.

[RequireComponent(typeof(Selectable))]
public class FieldValue : MonoBehaviour
{
    [FormerlySerializedAs("variable")]
    [Tooltip("The first non-null var ref below will take precedence")]
    [SerializeField] FloatVariable floatVariable;
    [SerializeField] BoolVariable boolVariable;
    [SerializeField] bool initializeOnAwake = true;
    [SerializeField] bool drawOnChange = false;

    NebulaGen.Nebula2 nebula2;

    Selectable selectable;
    TMP_InputField input;
    Slider slider;
    Toggle toggle;

    float GetValue()
    {
        return boolVariable != null ? (boolVariable.value ? 1f : 0f) :
        (floatVariable != null ? floatVariable.value : 0f);
    }

    void SetValue(float incoming)
    {
        if (boolVariable != null)
        {
            boolVariable.value = incoming > float.Epsilon;
            return;
        }
        if (floatVariable != null)
        {
            floatVariable.value = incoming;
            return;
        }
    }

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

        if (initializeOnAwake && floatVariable != null) floatVariable.ResetVariable();
        if (initializeOnAwake && boolVariable != null) boolVariable.ResetVariable();
        UpdateUI();
    }

    void UpdateUI()
    {
        float value = GetValue();
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
        SetValue(incoming);
        UpdateUI();
        nebula2.GenerateNoise();
        if (drawOnChange) nebula2.DrawOutput();
    }
}
