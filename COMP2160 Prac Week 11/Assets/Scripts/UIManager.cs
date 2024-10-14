using UnityEngine;
using UnityEngine.InputSystem;

// note this has to run earlier than other classes which subscribe to the TargetSelected event
[DefaultExecutionOrder(-100)]
public class UIManager : MonoBehaviour
{
#region UI Elements
    [SerializeField] private Transform crosshair;
    [SerializeField] private Transform target;
#endregion 

#region Settings
    [SerializeField] private bool useDeltaMovement = true; // Toggle between behaviors
#endregion

#region Singleton
    static private UIManager instance;
    static public UIManager Instance
    {
        get { return instance; }
    }
#endregion 

#region Actions
    private Actions actions;
    private InputAction mouseAction;
    private InputAction deltaAction;
    private InputAction selectAction;
#endregion

#region Events
    public delegate void TargetSelectedEventHandler(Vector3 worldPosition);
    public event TargetSelectedEventHandler TargetSelected;
#endregion

#region Init & Destroy
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one UIManager in the scene.");
        }

        instance = this;

        actions = new Actions();
        mouseAction = actions.mouse.position;
        deltaAction = actions.mouse.delta;
        selectAction = actions.mouse.select;

        Cursor.visible = false;
        target.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        actions.mouse.Enable();
    }

    void OnDisable()
    {
        actions.mouse.Disable();
    }
#endregion Init

#region Update
    void Update()
    {
        Debug.Log(deltaAction.ReadValue<Vector2>());
        MoveCrosshair();
        SelectTarget();
    }

    private void MoveCrosshair() 
    {
        if (useDeltaMovement)
        {
            // New behavior: move crosshair by delta
            MoveCrosshairByDelta();
        }
        else
        {
            // Original behavior: move crosshair to the mouse position
            MoveCrosshairToMousePosition();
        }

        // Make sure the crosshair stays within screen bounds
        ClampCrosshairToScreen();
    }

    private void MoveCrosshairByDelta()
    {
        // Get the mouse delta (change in screen space)
        Vector2 mouseDelta = deltaAction.ReadValue<Vector2>();
        
        float movementScale = 0.5f;  // Adjust this to make the crosshair move slower or faster
        mouseDelta *= movementScale;

        // Get the current screen position of the crosshair
        Vector3 currentScreenPosition = Camera.main.WorldToScreenPoint(crosshair.position);

        // Adjust the screen position by the mouse delta (move the crosshair by delta in screen space)
        Vector3 newScreenPosition = new Vector3(
            currentScreenPosition.x + mouseDelta.x,
            currentScreenPosition.y + mouseDelta.y,
            currentScreenPosition.z  // Keep the same Z distance from the camera
        );

        // Cast a ray from the camera through the updated screen position
        Ray ray = Camera.main.ScreenPointToRay(newScreenPosition);

        // Define a plane (assumed to be in the XZ plane with an upward normal vector representing the board)
        Plane boardPlane = new Plane(Vector3.up, Vector3.zero);  // Assuming the board is at y = 0

        // Perform the raycast to find where the new ray hits the board plane
        if (boardPlane.Raycast(ray, out float enter))
        {
            // Get the intersection point of the ray and the plane
            Vector3 hitPoint = ray.GetPoint(enter);

            // Move the crosshair to the intersection point
            crosshair.position = hitPoint;

            // Optionally, lock the mouse to center it on the crosshair after moving
            Cursor.lockState = CursorLockMode.Locked; // Keeps the mouse centered on the crosshair
        }
    }





    private void MoveCrosshairToMousePosition()
    {
        // Get the mouse position in screen space
        Vector2 mousePosition = mouseAction.ReadValue<Vector2>();

        // Create a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        // Define a plane in the XZ plane with an upward normal vector (representing the board)
        Plane boardPlane = new Plane(Vector3.up, Vector3.zero);  // Assuming the board is at y = 0

        // Perform a raycast to see if the ray intersects the plane
        if (boardPlane.Raycast(ray, out float enter))
        {
            // Get the intersection point of the ray and the plane
            Vector3 hitPoint = ray.GetPoint(enter);

            hitPoint.y += 0.1f;

            // Move the crosshair to the hit point
            crosshair.position = hitPoint;
        }
    }

    private void ClampCrosshairToScreen()
    {
        // Convert the crosshair's world position to screen space
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(crosshair.position);

        // Define a screen rectangle (assuming screen is 2D, with z=0)
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

        // Use Mathf.Clamp to clamp the position within screen bounds
        screenPosition.x = Mathf.Clamp(screenPosition.x, screenRect.xMin, screenRect.xMax);
        screenPosition.y = Mathf.Clamp(screenPosition.y, screenRect.yMin, screenRect.yMax);

        // Convert back to world coordinates and update crosshair position
        crosshair.position = Camera.main.ScreenToWorldPoint(screenPosition);
    }



    private void SelectTarget()
    {
        if (selectAction.WasPerformedThisFrame())
        {
            // set the target position and invoke 
            target.gameObject.SetActive(true);
            target.position = crosshair.position;     
            TargetSelected?.Invoke(target.position);       
        }
    }

#endregion Update

}
