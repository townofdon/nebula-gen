using UnityEngine;
using NebulaGen;
using CyberneticStudios.SOFramework;

public class SectionNoiseType : MonoBehaviour
{
    [SerializeField] NoiseType[] noiseTypes;
    [SerializeField] NoiseTypeVariable noiseType;

    void Awake()
    {
        // Event callbacks are defined in Awake,OnDestroy instead of OnEnable,OnDisable
        // so that the events can continue to be called even when this GO is inactive/disabled.
        noiseType.OnChanged += OnNoiseTypeChange;
    }

    void OnDestroy()
    {
        noiseType.OnChanged -= OnNoiseTypeChange;
    }

    void Start()
    {
        OnNoiseTypeChange(noiseType.value);
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
