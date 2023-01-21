using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class Modal : MonoBehaviour
{
    [SerializeField] bool showOnAwake = false;
    [SerializeField] Selectable buttonClose;
    [SerializeField] UnityEvent OnShow;
    [SerializeField] UnityEvent OnHide;

    EventSystem eventSystem;
    MainTabs tabs;
    Canvas canvas;

    public void Show()
    {
        eventSystem.SetSelectedGameObject(null);
        canvas.enabled = true;
        OnShow.Invoke();
        buttonClose.Select();
    }

    public void Hide()
    {
        canvas.enabled = false;
        OnHide.Invoke();
        tabs.Refocus();
    }

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        tabs = FindObjectOfType<MainTabs>();
        eventSystem = FindObjectOfType<EventSystem>();
        canvas.enabled = showOnAwake;
    }
}
