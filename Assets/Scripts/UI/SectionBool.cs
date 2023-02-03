using UnityEngine;
using CyberneticStudios.SOFramework;

public class SectionBool : MonoBehaviour
{
    [SerializeField] BoolVariable variable;

    void Awake()
    {
        variable.OnChanged += OnBorderModeChange;
    }

    void OnDestroy()
    {
        variable.OnChanged -= OnBorderModeChange;
    }

    void Start()
    {
        OnBorderModeChange(variable.value);
    }

    void OnBorderModeChange(bool incoming)
    {
        if (incoming)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    void Activate()
    {
        gameObject.SetActive(true);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
