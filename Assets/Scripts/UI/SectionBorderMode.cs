using UnityEngine;
using NebulaGen;

public class SectionBorderMode : MonoBehaviour
{
    [SerializeField] BorderMode[] borderModes;

    Nebula2 nebula2;

    void Awake()
    {
        nebula2 = FindObjectOfType<Nebula2>();
        nebula2.OnBorderModeChange += OnBorderModeChange;
    }

    void Start()
    {
        OnBorderModeChange(nebula2.CurrentBorderMode);
    }

    void OnDestroy()
    {
        nebula2.OnBorderModeChange -= OnBorderModeChange;
    }

    void OnBorderModeChange(BorderMode incoming)
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

    bool ContainsBorderMode(BorderMode incoming)
    {
        for (int i = 0; i < borderModes.Length; i++)
        {
            if (incoming == borderModes[i]) return true;
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
