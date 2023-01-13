using System;
using NebulaGen;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class ViewBase : MonoBehaviour
{
    [SerializeField] TabType tabType;
    [SerializeField] public bool activateOnAwake = true;

    protected MainTabs tabs;
    protected Nebula2 nebula2;
    protected SpriteBase spriteBackground;
    protected SpriteBase spriteDrawSurface;
    protected SpriteBase spriteNoise;
    protected SpriteBase spriteMask;
    protected SpriteBase spriteOutput;

    InputHandler input;
    Canvas canvas;
    FocusableField[] fields;
    int currentFieldIndex;

    protected void OnEnable()
    {
        tabs.OnTabChange += OnTabChange;
        input.OnTabForward += OnTabForward;
        input.OnTabBackward += OnTabBackward;
    }

    protected void OnDisable()
    {
        tabs.OnTabChange -= OnTabChange;
        input.OnTabForward -= OnTabForward;
        input.OnTabBackward -= OnTabBackward;
    }

    protected void Awake()
    {
        tabs = FindObjectOfType<MainTabs>();
        input = FindObjectOfType<InputHandler>();
        nebula2 = FindObjectOfType<Nebula2>();
        canvas = GetComponent<Canvas>();
        fields = GetComponentsInChildren<FocusableField>(true);
        spriteBackground = FindObjectOfType<SpriteBackground>(true);
        spriteDrawSurface = FindObjectOfType<SpriteDrawSurface>(true);
        spriteNoise = FindObjectOfType<SpriteNoise>(true);
        spriteMask = FindObjectOfType<SpriteMask>(true);
        spriteOutput = FindObjectOfType<SpriteOutput>(true);
    }

    protected virtual void OnActivate() { }

    void OnTabChange(TabType incoming)
    {
        if (incoming == tabType)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    void OnTabForward()
    {
        if (!canvas.enabled) return;
        GotoNextField(1);
    }

    void OnTabBackward()
    {
        if (!canvas.enabled) return;
        GotoNextField(-1);
    }

    void Activate()
    {
        canvas.enabled = true;
        FocusOnFirstField();
        OnActivate();
    }

    void Deactivate()
    {
        canvas.enabled = false;
    }

    void FocusOnFirstField()
    {
        for (int i = 0; i < fields.Length; i++)
        {
            currentFieldIndex = i;
            if (fields[currentFieldIndex].interactable)
            {
                fields[currentFieldIndex].Select();
                return;
            }
        }
    }

    void GotoNextField(int direction = 1)
    {
        for (int i = 0; i < fields.Length; i++)
        {
            currentFieldIndex += direction;
            currentFieldIndex %= fields.Length;
            if (currentFieldIndex == -1) currentFieldIndex = fields.Length - 1;
            if (fields[currentFieldIndex].interactable)
            {
                fields[currentFieldIndex].Select();
                return;
            }
        }
    }
}
