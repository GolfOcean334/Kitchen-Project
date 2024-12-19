using UnityEngine;
using UnityEngine.InputSystem;

public class DropItem : MonoBehaviour
{
    [SerializeField] private ChangeMainHand mainHand;
    [SerializeField] private HighlightHandler highlightHandler;
    [SerializeField] private PickupHandler pickupHandler;
    [SerializeField] private Camera playerCamera;

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
            TryPlaceOrDropObject();
        }
    }
    private void TryPlaceOrDropObject()
    {
        GameObject handObject = pickupHandler.GetCurrentMainHandObject();

        if (handObject == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            PlaceObject(handObject, hit.point);
        }
        else
        {
            DropMainHandObject();
        }
    }

    private void PlaceObject(GameObject handObject, Vector3 targetPosition)
    {
        targetPosition.y += 0.2f;

        pickupHandler.ClearCurrentMainHandObject();

        handObject.transform.SetParent(null);
        handObject.transform.position = targetPosition;

        Debug.Log($"Objet {handObject.name} placé à {targetPosition}");
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
