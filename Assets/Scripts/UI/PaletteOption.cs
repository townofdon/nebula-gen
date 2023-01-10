using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using ColorPalette;
using NebulaGen;
using UnityEngine.EventSystems;

public class PaletteOption : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] Palette palette;
    [Space]
    [Space]
    [SerializeField] TMPro.TextMeshProUGUI colorLabel;
    [SerializeField] GameObject colorsContainer;
    [SerializeField] GameObject selectedIcon;
    [SerializeField] GameObject deselectedMask;
    [SerializeField] GameObject focusedOutline;

    Button button;
    Nebula2 nebula2;

    public void OnSelect(BaseEventData eventData)
    {
        focusedOutline.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        focusedOutline.SetActive(false);
    }

    void OnEnable()
    {
        button.onClick.AddListener(OnClick);
        nebula2.OnPaletteChange += OnPaletteChange;
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
        nebula2.OnPaletteChange -= OnPaletteChange;
    }

    void OnClick()
    {
        button.Select();
        nebula2.SetPalette(palette);
        nebula2.DrawOutput();
    }

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        nebula2 = FindObjectOfType<Nebula2>();
        Assert.IsNotNull(colorLabel);
        Assert.IsNotNull(button);
        Assert.IsNotNull(selectedIcon);
        Assert.IsNotNull(deselectedMask);
        Assert.IsNotNull(colorsContainer);
        Assert.IsNotNull(palette);
        Assert.IsNotNull(focusedOutline);
        SetupColorsFromPalette();
        OnPaletteChange(nebula2.MainPalette);
        focusedOutline.SetActive(false);
    }

    void SetupColorsFromPalette()
    {
        Transform firstChildCopy = Instantiate(colorsContainer.transform.GetChild(0), transform.position, Quaternion.identity);
        foreach (Transform child in colorsContainer.transform) Destroy(child.gameObject);
        foreach (var color in palette.GetColors())
        {
            var instance = Instantiate(firstChildCopy, colorsContainer.transform);
            instance.GetComponent<Image>().color = color;
        }
        Destroy(firstChildCopy.gameObject);
        colorLabel.text = palette.GetName();
    }

    void OnPaletteChange(Palette incoming)
    {
        if (incoming == palette)
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
        selectedIcon.SetActive(true);
        deselectedMask.SetActive(false);
    }

    void Deactivate()
    {
        selectedIcon.SetActive(false);
        deselectedMask.SetActive(true);
    }
}
