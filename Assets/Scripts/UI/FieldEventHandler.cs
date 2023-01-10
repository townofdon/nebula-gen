using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FieldEventHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] bool debug;
    public UnityEvent OnSelected;
    public UnityEvent OnDeselected;

    public void OnSelect(BaseEventData eventData)
    {
        if (debug) Debug.Log($"{gameObject.name} selected");
        OnSelected?.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (debug) Debug.Log($"{gameObject.name} de-selected");
        OnDeselected?.Invoke();
    }
}
