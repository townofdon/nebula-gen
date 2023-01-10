using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SliderVal : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI displayText;

    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        Assert.IsNotNull(displayText);
    }

    void Start()
    {
        OnValueChanged(slider.value);
    }

    void OnEnable()
    {
        slider.onValueChanged.AddListener(OnValueChanged);
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    void OnValueChanged(float incomingValue)
    {
        displayText.text = ((int)(incomingValue * 100f) / 100f).ToString();
    }
}
