using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using CyberneticStudios.SOFramework;

[RequireComponent(typeof(Button))]
public class TextureSelect : MonoBehaviour
{
    [SerializeField] Image focusOutline;

    Button button;
    RawImage image;

    TexturePicker texturePicker;
    Texture2DVariable textureVar;

    void OnEnable()
    {
        button.onClick.AddListener(OnClick);
        textureVar.OnChanged += OnTextureChanged;
        OnTextureChanged(texturePicker.textureVar.value);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
        textureVar.OnChanged -= OnTextureChanged;
    }

    void Awake()
    {
        button = GetComponent<Button>();
        texturePicker = GetComponentInParent<TexturePicker>(true);
        image = GetComponentInChildren<RawImage>(true);
        Assert.IsNotNull(image);
        Assert.IsNotNull(image.texture);
        Assert.IsNotNull(texturePicker);
        Assert.IsNotNull(texturePicker.textureVar);
        textureVar = texturePicker.textureVar;
        HideOutline();
    }

    void OnClick()
    {
        texturePicker.SelectTexture((Texture2D)image.texture);
    }

    void OnTextureChanged(Texture2D incoming)
    {
        if (incoming != null && incoming == image.texture)
        {
            ShowOutline();
        }
        else
        {
            HideOutline();
        }
    }

    void ShowOutline()
    {
        if (focusOutline != null) focusOutline.SetActiveAndEnable(true);
    }

    void HideOutline()
    {
        if (focusOutline != null) focusOutline.SetActiveAndEnable(false);
    }
}
