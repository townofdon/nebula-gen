
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


public class MainTab : MonoBehaviour
{
    [SerializeField] TabType tab;
    Button button;

    MainTabs main;

    Image mainImage;
    Sprite mainSprite;
    Sprite pressedSprite;
    Sprite selectedSprite;
    Sprite highlightedSprite;
    Sprite disabledSprite;

    void OnEnable()
    {
        button.onClick.AddListener(OnClick);
        main.OnTabChange += OnTabChange;
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
        main.OnTabChange -= OnTabChange;
    }

    void Awake()
    {
        button = GetComponent<Button>();
        main = GetComponentInParent<MainTabs>();
        mainImage = GetComponent<Image>();
        Assert.IsNotNull(button);
        Assert.IsNotNull(main);
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

    // void Start()
    // {
    //     if (main.InitialTab == tab) Activate();
    // }

    void OnClick()
    {
        Activate();
        main.ChangeTab(tab);
    }

    void OnTabChange(TabType incoming)
    {
        if (incoming == tab)
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
        // button.Select();
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
