using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TexturePickerNavButton : MonoBehaviour
{

    [SerializeField] CustomTextureType customTextureType;

    Button button;
    Image mainImage;
    Sprite mainSprite;
    Sprite pressedSprite;
    Sprite selectedSprite;
    Sprite highlightedSprite;
    Sprite disabledSprite;

    TexturePicker texturePicker;

    void OnEnable()
    {
        button.onClick.AddListener(OnClick);
        texturePicker.OnSelectTextureType += OnSelectTexture;
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
        texturePicker.OnSelectTextureType -= OnSelectTexture;
    }

    void Awake()
    {
        texturePicker = GetComponentInParent<TexturePicker>(true);
        Assert.IsNotNull(texturePicker);

        mainImage = GetComponent<Image>();
        button = GetComponent<Button>();
        Assert.IsNotNull(button);
        Assert.IsNotNull(mainImage);
        mainSprite = mainImage.sprite;
        pressedSprite = button.spriteState.pressedSprite;
        selectedSprite = button.spriteState.selectedSprite;
        highlightedSprite = button.spriteState.highlightedSprite;
        disabledSprite = button.spriteState.disabledSprite;
        Assert.IsNotNull(button.spriteState.pressedSprite);
        Assert.IsNotNull(button.spriteState.selectedSprite);
        Assert.IsNotNull(button.spriteState.highlightedSprite);
        Assert.IsNotNull(button.spriteState.disabledSprite);
    }

    void OnClick()
    {
        Activate();
        texturePicker.SelectType(customTextureType);
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
        mainImage.sprite = selectedSprite;
        button.spriteState = new SpriteState
        {
            pressedSprite = selectedSprite,
            highlightedSprite = selectedSprite,
            selectedSprite = selectedSprite,
            disabledSprite = disabledSprite,
        };
    }

    void Deactivate()
    {
        mainImage.sprite = mainSprite;
        button.spriteState = new SpriteState
        {
            pressedSprite = pressedSprite,
            highlightedSprite = highlightedSprite,
            selectedSprite = selectedSprite,
            disabledSprite = disabledSprite,
        };
    }
}
