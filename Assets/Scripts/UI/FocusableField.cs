using UnityEngine;
using UnityEngine.UI;

public class FocusableField : MonoBehaviour
{
    Selectable selectable;
    RectTransform rectTransform;

    public bool interactable => selectable.interactable && selectable.enabled && isActiveAndEnabled;

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public void Select()
    {
        selectable.Select();
        rectTransform = GetComponent<RectTransform>();
    }

    void Awake()
    {
        selectable = GetComponent<Selectable>();
    }
}
