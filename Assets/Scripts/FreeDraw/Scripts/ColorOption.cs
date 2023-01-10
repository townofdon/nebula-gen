
using UnityEngine;
using UnityEngine.UI;

using FreeDraw;

public class ColorOption : MonoBehaviour
{
    [SerializeField] PenColorType color;
    [SerializeField] Image background;
    [SerializeField] Color activeColor = new Color(1f, .7f, 0);

    Color initialColorBG;

    DrawingSettings settings;

    void OnEnable()
    {
        settings.OnColorChange += OnColorChange;
    }

    void OnDisable()
    {
        settings.OnColorChange -= OnColorChange;
    }

    void Awake()
    {
        settings = FindObjectOfType<DrawingSettings>();
    }

    void Start()
    {
        initialColorBG = background.color;
        if (color == PenColorType.Black) Activate();
    }

    void OnColorChange(PenColorType newColor)
    {
        if (color == newColor)
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
        background.color = activeColor;
    }

    void Deactivate()
    {
        background.color = initialColorBG;
    }
}
