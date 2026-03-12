using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputReader : MonoBehaviour
{
    private Mobile inputActions;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    [SerializeField] public PlayerMovement playerMovement;

    [SerializeField] private Transform swipeTrailObject;
    [SerializeField] private TrailRenderer swipeTrail;
    [SerializeField] private Camera mainCamera;

    public int minSwipeDistance = 50;

    private void Awake()
    {
        inputActions = new Mobile();
        if (mainCamera == null) { 
            mainCamera = Camera.main;
        }
    }

    private void Start()
    {
        if (swipeTrail != null)
        {
            swipeTrail.gameObject.SetActive(false);
        }
        
    }
    private Vector3 ScreenToWorldPoint(Vector2 screenPosition)
    {
        Vector3 screenPoint = new Vector3(screenPosition.x, screenPosition.y, 10f);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        worldPoint.z = 0f;
        return worldPoint;
    }

   
    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Touch.PrimaryContact.started += OnTouchStarted;
        inputActions.Touch.PrimaryContact.canceled += OnTouchEnded;
    }

    private void OnDisable()
    {
        inputActions.Touch.PrimaryContact.started -= OnTouchStarted;
        inputActions.Touch.PrimaryContact.canceled -= OnTouchEnded;

        inputActions.Disable();
    }
    

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        startTouchPosition = inputActions.Touch.PrimaryPosition.ReadValue<Vector2>();
        Debug.Log("Start Position: " + startTouchPosition);

        if (swipeTrail != null)
        {
            swipeTrail.Clear();
        }
        
        if (swipeTrailObject != null)
        {
            Vector3 startWorld = ScreenToWorldPoint(startTouchPosition);
            swipeTrailObject.position =  startWorld;
            swipeTrailObject.gameObject.SetActive(true);
        }
    }
    
    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        endTouchPosition = inputActions.Touch.PrimaryPosition.ReadValue<Vector2>();
        Debug.Log("End Position: " + endTouchPosition);
        if (swipeTrail != null)
        {
            swipeTrail.gameObject.SetActive(false);
        }
        DetectSwipe();
    }

    private void Update()
    {
        bool isTouching = inputActions.Touch.PrimaryContact.IsPressed();

        if (isTouching && swipeTrailObject != null && swipeTrailObject.gameObject.activeSelf)
        {
            Vector2 currentTouchPosition = inputActions.Touch.PrimaryPosition.ReadValue<Vector2>();
            Vector3 currentWorld = ScreenToWorldPoint(currentTouchPosition);

            swipeTrailObject.position = currentWorld;
        }

    }
    private void DetectSwipe()
    {
        Vector2 swipeData = endTouchPosition - startTouchPosition;
        if (swipeData.magnitude < minSwipeDistance)
        {
            Debug.Log("Swipe Short");playerMovement.SwipeMoveStop(); return;
        }
        if (Mathf.Abs(swipeData.x) > Mathf.Abs(swipeData.y))
        {
            if (swipeData.x > 0)
            {
                Debug.Log("Swipe Right");
                playerMovement.SwipeMoveRight();
            }
            else
            {
                Debug.Log("Swipe Left");
                playerMovement.SwipeMoveLeft();
            }
        } else 
        {
            if (swipeData.y > 0)
            {
                Debug.Log("Swipe Up");
                playerMovement.SwipeMoveJump();
            }
            else
            {
                Debug.Log("Swipe Down");
            }
        }
    }
}
