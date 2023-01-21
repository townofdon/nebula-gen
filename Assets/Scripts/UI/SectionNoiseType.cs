using UnityEngine;
using NebulaGen;

public class SectionNoiseType : MonoBehaviour
{
    [SerializeField] NoiseType[] noiseTypes;

    Nebula2 nebula2;

    void Awake()
    {
        nebula2 = FindObjectOfType<Nebula2>();
        // Event callbacks are defined in Awake,OnDestroy instead of OnEnable,OnDisable
        // so that the events can continue to be called even when this GO is inactive/disabled.
        nebula2.OnNoiseTypeChange += OnNoiseTypeChange;
    }

    void Start()
    {
        OnNoiseTypeChange(nebula2.CurrentNoiseType);
    }

    void OnDestroy()
    {
        nebula2.OnNoiseTypeChange -= OnNoiseTypeChange;
    }

    void OnNoiseTypeChange(NoiseType incoming)
    {
        if (ContainsBorderMode(incoming))
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    bool ContainsBorderMode(NoiseType incoming)
    {
        for (int i = 0; i < noiseTypes.Length; i++)
        {
            if (incoming == noiseTypes[i]) return true;
        }
        return false;
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
