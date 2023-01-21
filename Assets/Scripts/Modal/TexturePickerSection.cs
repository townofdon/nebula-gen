using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TexturePickerSection : MonoBehaviour
{
    [SerializeField] CustomTextureType customTextureType;
    [SerializeField] Texture2D[] textures;

    RawImage[] images;

    TexturePicker texturePicker;

    void Awake()
    {
        texturePicker = GetComponentInParent<TexturePicker>();
        Assert.IsNotNull(texturePicker);
        texturePicker.OnSelectTextureType += OnSelectTexture;
        PopulatePreviewImages();
    }

    void OnDestroy()
    {
        texturePicker.OnSelectTextureType -= OnSelectTexture;
    }

    void OnSelectTexture(CustomTextureType incoming)
    {
        if (incoming == customTextureType)
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

    void PopulatePreviewImages()
    {
        if (images == null) images = GetComponentsInChildren<RawImage>();
        if (images == null) { Debug.LogWarning($"No images found in TexturePickerSection \"{gameObject.name}\""); }
        if (images.Length < textures.Length) Debug.LogWarning($"Not enough images to accommodate customTextures[]. images={images.Length}, textures={textures.Length}");
        for (int i = 0; i < images.Length; i++)
        {
            if (i < textures.Length)
            {
                images[i].texture = textures[i];
            }
            else
            {
                images[i].gameObject.SetActive(false);
            }
        }
    }
}
