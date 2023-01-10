using UnityEngine;
using UnityEngine.UI;

public class FocusableField : MonoBehaviour
{
    Selectable selectable;

    public bool interactable => selectable.interactable && selectable.enabled && isActiveAndEnabled;

    public void Select()
    {
        selectable.Select();
    }

    void Awake()
    {
        selectable = GetComponent<Selectable>();
    }
}
