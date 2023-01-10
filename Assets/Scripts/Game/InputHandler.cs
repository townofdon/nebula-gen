using System;
using UnityEngine;
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

    void OnTabChange(TabType incomingTab)
    {
        if (tabs.ShouldViewReset) ResetCamera();
    }

    void HandleMove()
    {
        if (!tabs.CanUserMove) return;
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
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
        if (shouldZoomIn) { zoom -= 1f; }
        if (shouldZoomOut) { zoom += 1f; }
        camera.orthographicSize += zoom * zoomSpeed * Time.deltaTime;
        if (camera.orthographicSize < minZoom) camera.orthographicSize = minZoom;
    }

    void HandleSave()
    {
        bool shouldSave = Input.GetKeyDown(KeyCode.G);
        if (!shouldSave) return;
        nebula2.SaveImage();
    }

    void HandleReset()
    {
        if (!Input.GetKeyDown(KeyCode.R)) return;
        ResetCamera();
    }
}
