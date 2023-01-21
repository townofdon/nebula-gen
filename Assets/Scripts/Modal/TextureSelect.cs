using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TextureSelect : MonoBehaviour
{
    Button button;
    RawImage image;

    TexturePicker texturePicker;

    void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    void Awake()
    {
        button = GetComponent<Button>();
        texturePicker = GetComponentInParent<TexturePicker>(true);
        image = GetComponentInChildren<RawImage>(true);
        Assert.IsNotNull(image);
        Assert.IsNotNull(image.texture);
        Assert.IsNotNull(texturePicker);
    }

    void OnClick()
    {
        texturePicker.SelectTexture((Texture2D)image.texture);
    }
}
