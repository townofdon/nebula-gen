using System;
using UnityEngine;
using UnityEngine.EventSystems;
using NebulaGen;

public class InputHandler : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 2.0f;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] new Camera camera;
    [SerializeField] float minZoom = 1f;

    Vector3 move;
    float zoom;

    Vector3 initialCameraPosition;
    float initialCameraZoom;

    EventSystem eventSystem;
    MainTabs tabs;
    Nebula2 nebula2;

    public Action OnTabForward;
    public Action OnTabBackward;

    public void ResetCamera()
    {
        camera.transform.position = initialCameraPosition;
        camera.orthographicSize = initialCameraZoom;
    }

    void OnEnable()
    {
        tabs.OnTabChange += OnTabChange;
    }

    void OnDisable()
    {
        tabs.OnTabChange -= OnTabChange;
    }

    void Awake()
    {
        tabs = FindObjectOfType<MainTabs>();
        nebula2 = FindObjectOfType<Nebula2>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    void Start()
    {
        initialCameraPosition = camera.transform.position;
        initialCameraZoom = camera.orthographicSize;
    }

    void Update()
    {
        HandleMove();
        HandleZoom();
        HandleTab();
        HandleNumberShortcut();
        HandleReset();
        HandleSave();
    }

    void HandleTab()
    {
        bool isShiftKeyHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (!Input.GetKeyDown(KeyCode.Tab)) return;
        if (isShiftKeyHeld)
        {
            OnTabBackward?.Invoke();
        }
        else
        {
            OnTabForward?.Invoke();
        }
    }

    void HandleNumberShortcut()
    {
        bool isShiftKeyHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (!isShiftKeyHeld) return;
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            tabs.ChangeTab(TabType.Main);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            tabs.ChangeTab(TabType.Noise);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            tabs.ChangeTab(TabType.Mask);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            tabs.ChangeTab(TabType.Border);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            tabs.ChangeTab(TabType.Help);
        }
    }

    void OnTabChange(TabType incomingTab)
    {
        if (tabs.ShouldViewReset) ResetCamera();
    }

    void HandleMove()
    {
        if (!tabs.CanUserMove) return;
        float vertical = IsControlPressed() ? 0 : Input.GetAxisRaw("Vertical");
        float horizontal = IsControlPressed() ? 0 : Input.GetAxisRaw("Horizontal");
        move = camera.transform.position;
        move.x += horizontal * scrollSpeed * camera.orthographicSize * Time.deltaTime;
        move.y += vertical * scrollSpeed * camera.orthographicSize * Time.deltaTime;
        camera.transform.position = move;
    }

    void HandleZoom()
    {
        if (!tabs.CanUserMove) return;
        zoom = 0f;
        bool shouldZoomIn = Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Equals);
        bool shouldZoomOut = Input.GetKey(KeyCode.Underscore) || Input.GetKey(KeyCode.Minus);
        if (shouldZoomIn || shouldZoomOut) DeselectCurrentlyFocused();
        if (shouldZoomIn) { zoom -= 1f; }
        if (shouldZoomOut) { zoom += 1f; }
        camera.orthographicSize += zoom * zoomSpeed * Time.deltaTime;
        if (camera.orthographicSize < minZoom) camera.orthographicSize = minZoom;
    }

    void HandleSave()
    {
        if (!tabs.CanSave) return;
        bool shouldSave = Input.GetKeyDown(KeyCode.S) && IsControlPressed();
        if (!shouldSave) return;
        nebula2.SaveImage();
    }

    bool IsControlPressed()
    {
        return Input.GetKey(KeyCode.LeftControl) ||
        Input.GetKey(KeyCode.RightControl) ||
        Input.GetKey(KeyCode.LeftCommand) ||
        Input.GetKey(KeyCode.RightCommand);
    }

    void HandleReset()
    {
        if (!Input.GetKeyDown(KeyCode.R)) return;
        ResetCamera();
    }

    void DeselectCurrentlyFocused()
    {
        eventSystem.SetSelectedGameObject(null);
    }
}
