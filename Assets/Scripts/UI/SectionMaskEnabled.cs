using UnityEngine;
using NebulaGen;

public class SectionMaskEnabled : MonoBehaviour
{
    Nebula2 nebula2;

    void Awake()
    {
        nebula2 = FindObjectOfType<Nebula2>();
        // Event callbacks are defined in Awake,OnDestroy instead of OnEnable,OnDisable
        // so that the events can continue to be called even when this GO is inactive/disabled.
        nebula2.OnMaskEnabledChange += OnMaskEnabledChange;
    }

    void Start()
    {
        OnMaskEnabledChange(nebula2.IsMaskEnabled);
    }

    void OnDestroy()
    {
        nebula2.OnMaskEnabledChange -= OnMaskEnabledChange;
    }

    void OnMaskEnabledChange(bool enabled)
    {
        if (enabled)
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
