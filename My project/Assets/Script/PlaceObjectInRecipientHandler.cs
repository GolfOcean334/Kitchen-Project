using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceObjectInRecipientHandler : MonoBehaviour
{
    [SerializeField] private PickupHandler pickupHandler;
    [SerializeField] private ChangeMainHand changeMainHand;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 3.0f;

    private PlayerInput playerInput;
    private InputAction placeAction;

    private void Awake()
    {
        playerInput = new PlayerInput();
        placeAction = playerInput.Player.PlaceObjectInRecipient;
    }

    private void OnEnable()
    {
        placeAction.Enable();
    }

    private void OnDisable()
    {
        placeAction.Disable();
    }
    private void Update()
    {
        if (placeAction.triggered) 
        {
            TryPlaceObject();
        }

    }

    private void TryPlaceObject()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Recipient"))
            {
                GameObject handObject = pickupHandler.GetCurrentMainHandObject();

                if (handObject != null)
                {
                    PlaceObjectInRecipient(hit.collider.gameObject, handObject);
                    pickupHandler.ClearCurrentMainHandObject();
                }
            }
        }
    }
    private void PlaceObjectInRecipient(GameObject recipient, GameObject handObject)
    {
        Debug.Log($"Tentative de placement de {handObject.name} dans {recipient.name}");

        if (handObject == null || recipient == null)
        {
            Debug.LogError("L'objet ou le récipient est nul !");
            return;
        }

        Vector3 originalScale = handObject.transform.localScale;

        Rigidbody rb = handObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        Collider[] colliders = handObject.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        Vector3 targetPosition = recipient.transform.position + recipient.transform.up * 0.1f;
        handObject.transform.position = targetPosition;

        handObject.transform.rotation = recipient.transform.rotation;
        handObject.transform.localScale = Vector3.one;

        Debug.Log("Objet placé, suppression de la main principale...");
        pickupHandler.ClearCurrentMainHandObject();

        handObject.transform.localScale = originalScale;
    }

}
