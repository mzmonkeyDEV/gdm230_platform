using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputReader : MonoBehaviour
{
    private Mobile inputActions;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    public int minSwipeDistance = 50;

    private void Awake()
    {
        inputActions = new Mobile();
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
    }
    
    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        endTouchPosition = inputActions.Touch.PrimaryPosition.ReadValue<Vector2>();
        Debug.Log("End Position: " + endTouchPosition);
        DetectSwipe();
    }
    private void DetectSwipe()
    {
        Vector2 swipeData = endTouchPosition - startTouchPosition;
        if (swipeData.magnitude < minSwipeDistance)
        {
            Debug.Log("Swipe Short"); return;
        }
        if (Mathf.Abs(swipeData.x) > Mathf.Abs(swipeData.y))
        {
            if (swipeData.x > 0)
            {
                Debug.Log("Swipe Right");
            }
            else
            {
                Debug.Log("Swipe Left");
            }
        } else 
        {
            if (swipeData.y > 0)
            {
                Debug.Log("Swipe Up");
            }
            else
            {
                Debug.Log("Swipe Down");
            }
        }
    }
}
