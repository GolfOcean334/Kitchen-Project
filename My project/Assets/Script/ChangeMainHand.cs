using UnityEngine;
using UnityEngine.InputSystem;

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
            TrySetMainHand(0);
        }
        else if (scrollDelta.y < 0)
        {
            TrySetMainHand(1);
        }
    }

    public void TrySetMainHand(int handIndex)
    {
        if (currentMainHandIndex == handIndex)
            return;

        GameObject rightHandObject = pickupHandler.GetRightHandObject();
        GameObject leftHandObject = pickupHandler.GetLeftHandObject();

        if (handIndex == 0 && rightHandObject == null)
            return;

        if (handIndex == 1 && leftHandObject == null)
            return;

        SetMainHand(handIndex);
    }

    public void SetMainHand(int handIndex)
    {
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
