using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    // Define the zoom speed and the min/max zoom limits
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;

    private Camera cam;
    private InputAction zoomAction;

    private void Awake()
    {
        cam = Camera.main;

        // Assuming 'actions' is a reference to your Input Actions object
        zoomAction = new Actions().camera.zoom;
    }

    private void OnEnable()
    {
        zoomAction.Enable();
    }

    private void OnDisable()
    {
        zoomAction.Disable();
    }

    private void Update()
    {
        float zoomInput = zoomAction.ReadValue<float>();

        if (cam.orthographic)
        {
            // Adjust orthographic size for zoom in an orthographic camera
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomInput * zoomSpeed, minZoom, maxZoom);
        }
        else
        {
            // Adjust field of view for zoom in a perspective camera
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - zoomInput * zoomSpeed, minZoom, maxZoom);
        }
    }
}