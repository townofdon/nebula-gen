using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    ViewBase[] views;

    void Awake()
    {
        views = FindObjectsOfType<ViewBase>(true);
    }

    void Start()
    {
        foreach (var view in views)
        {
            if (view.activateOnAwake) view.gameObject.SetActive(true);
        }
    }
}