using UnityEngine;
using UnityEngine.InputSystem;

public class DropItem : MonoBehaviour
{
    [SerializeField] private ChangeMainHand mainHand;
    [SerializeField] private HighlightHandler highlightHandler;
    [SerializeField] private PickupHandler pickupHandler;

    private PlayerInput playerInput;
    private InputAction dropMainHandAction;

    private void Awake()
    {
        playerInput = new PlayerInput();
        dropMainHandAction = playerInput.Player.Drop;
    }
    private void OnEnable()
    {
        dropMainHandAction.Enable();
    }
    private void OnDisable()
    {
        dropMainHandAction.Disable();
    }
    private void Update()
    {
        if (dropMainHandAction.triggered)
        {
            DropMainHandObject();
        }
    }
    private void DropMainHandObject()
    {
        if (mainHand.currentMainHandObject == null)
            return;

        highlightHandler.RemoveHighlight(mainHand.currentMainHandObject);

        Vector3 dropPosition = mainHand.currentMainHandObject.transform.position;

        dropPosition.y += 0.5f;

        mainHand.currentMainHandObject.transform.SetParent(null);

        mainHand.currentMainHandObject.transform.position = dropPosition;

        Rigidbody rb = mainHand.currentMainHandObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (mainHand.currentMainHandIndex == 0)
        {
            pickupHandler.ClearRightHandObject();
        }
        else
        {
            pickupHandler.ClearLeftHandObject();
        }

        mainHand.currentMainHandObject = null;
    }
}
