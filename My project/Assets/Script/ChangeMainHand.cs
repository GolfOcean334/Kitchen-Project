using UnityEngine.InputSystem;
using UnityEngine;

public class ChangeMainHand : MonoBehaviour
{
    [SerializeField] private PickupHandler pickupHandler;
    [SerializeField] private HighlightHandler highlightHandler;

    public int currentMainHandIndex = 0;
    public GameObject currentMainHandObject;

    private PlayerInput playerInput;
    private InputAction changeMainHandAction;

    private void Awake()
    {
        playerInput = new PlayerInput();
        changeMainHandAction = playerInput.Player.ChangeMainHand;
    }
    private void OnEnable()
    {
        changeMainHandAction.Enable();
    }
    private void OnDisable()
    {
        changeMainHandAction.Disable();
    }
    private void Update()
    {
        Vector2 scrollDelta = changeMainHandAction.ReadValue<Vector2>();

        if (scrollDelta.y > 0)
        {
            SetMainHand(0);
        }
        else if (scrollDelta.y < 0)
        {
            SetMainHand(1);
        }
    }

    public void SetMainHand(int handIndex)
    {
        if (handIndex == 0 && pickupHandler.GetRightHandObject() == null)
        {
            currentMainHandObject = null;
            currentMainHandIndex = -1;
            return;
        }

        if (handIndex == 1 && pickupHandler.GetLeftHandObject() == null)
        {
            currentMainHandObject = null;
            currentMainHandIndex = -1;
            return;
        }

        if (currentMainHandObject != null)
        {
            highlightHandler.RemoveHighlight(currentMainHandObject);
        }

        currentMainHandIndex = handIndex;

        if (currentMainHandIndex == 0)
        {
            currentMainHandObject = pickupHandler.GetRightHandObject();
        }
        else
        {
            currentMainHandObject = pickupHandler.GetLeftHandObject();
        }

        if (currentMainHandObject != null)
        {
            highlightHandler.ApplyHighlight(currentMainHandObject);
        }
    }

}