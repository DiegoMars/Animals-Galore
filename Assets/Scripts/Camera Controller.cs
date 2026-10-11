using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    // If we want to select an item to follow, inside the item script add:
    // public void OnMouseDown(){
    //   CameraController.instance.followTransform = transform;
    // }

    [Header("General")]
    [SerializeField] Transform cameraTransform;
    public Transform followTransform;
    Vector3 newPosition;
    Vector3 dragStartPosition;
    Vector3 dragCurrentPosition;

    [Header("Zoom")]
    [SerializeField] float zoomSpeed = 30f;
    [SerializeField] float minFieldOfView = 30f;
    [SerializeField] float maxFieldOfView = 100f;

    private Camera actualCamera;

    float fastSpeed = 0.05f;
    float normalSpeed = 0.02f;
    float movementSpeed;

    float edgeSize = 50f;
    bool isCursorSet = false;

    private void Start()
    {
        newPosition = transform.position;
        movementSpeed = normalSpeed;
        actualCamera = cameraTransform.GetComponent<Camera>();
    }

    private void Update()
    {
        // Allow Camera to follow Target
        if (followTransform != null)
        {
            transform.position = followTransform.position;
        }
        // Let us control Camera
        else
        {
            HandleCameraMovement();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    void HandleCameraMovement()
    {
        HandleMouseDragInput();
        HandleKeyboardInput();
        HandleZoomInput();
        // HandleEdgeScrolling();

        transform.position = newPosition;
        Cursor.lockState = CursorLockMode.Confined; // To not exit screen bounds
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += transform.right * -movementSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += transform.right * movementSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += transform.forward * movementSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += transform.forward * -movementSpeed;
        }
    }

    private void HandleEdgeScrolling()
    {
        // Move Right
        if (Input.mousePosition.x > Screen.width - edgeSize)
        {
            newPosition += transform.forward * movementSpeed;
            isCursorSet = true;
        }
        // Move Left
        else if (Input.mousePosition.x < edgeSize)
        {
            newPosition += transform.forward * -movementSpeed;
            isCursorSet = true;
        }
        // Move Up
        else if (Input.mousePosition.y > Screen.height - edgeSize)
        {
            newPosition += transform.right * -movementSpeed;
            isCursorSet = true;
        }
        // Move Down
        else if (Input.mousePosition.y < edgeSize)
        {
            newPosition += transform.right * movementSpeed;
            isCursorSet = true;
        }
        else
        {
            if (isCursorSet)
            {
                isCursorSet = false;
            }
        }
    }

    private void HandleMouseDragInput()
    {
        if (Input.GetMouseButtonDown(2) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
 
            float entry;
 
            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(2) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
    }

    private void HandleZoomInput()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollInput) < 0.01f)
        {
            return;
        }

        actualCamera.fieldOfView -= scrollInput * zoomSpeed;

        actualCamera.fieldOfView = Mathf.Clamp(
            actualCamera.fieldOfView,
            minFieldOfView,
            maxFieldOfView
        );
    }
}
