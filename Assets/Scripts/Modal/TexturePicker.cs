using System;
using UnityEngine;
using UnityEngine.Events;

using CyberneticStudios.SOFramework;

public enum CustomTextureType
{
    BOXES,
    CELLS,
    FIRE,
    GEOMETRIC,
    MOUNTAINS,
    SIMPLE,
    STAINEDGLASS,
    SWIRLS,
    VINES,
    WAVES,
}

public class TexturePicker : MonoBehaviour
{
    [SerializeField] Texture2DVariable textureVar;

    public Action<CustomTextureType> OnSelectTextureType;

    public UnityEvent OnSelectTexture;

    public void SelectTexture(Texture2D texture)
    {
        textureVar.value = texture;
        OnSelectTexture.Invoke();
    }

    public void SelectType(CustomTextureType type)
    {
        OnSelectTextureType?.Invoke(type);
    }

    void Awake()
    {
        var sections = GetComponentsInChildren<TexturePickerSection>(true);
        foreach (var section in sections)
        {
            section.gameObject.SetActive(true);
            section.enabled = true;
        }
    }

    void Start()
    {
        SelectType(CustomTextureType.BOXES);
    }
}
