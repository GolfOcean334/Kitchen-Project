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
        placeAction.performed += OnPlacePerformed;
    }

    private void OnDisable()
    {
        placeAction.Disable();
        placeAction.performed -= OnPlacePerformed;
    }

    private void OnPlacePerformed(InputAction.CallbackContext context)
    {
        TryPlaceObject();
    }

    private void TryPlaceObject()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Recipient"))
            {
                GameObject handObject = GetCurrentMainHandObject();

                if (handObject != null)
                {
                    PlaceObjectInRecipient(hit.collider.gameObject, handObject);
                    ClearCurrentMainHandObject();
                }
                else
                {
                    Debug.Log("Votre main principale est vide !");
                }
            }
        }
    }

    private void PlaceObjectInRecipient(GameObject recipient, GameObject handObject)
    {
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

        handObject.transform.SetParent(recipient.transform);

        handObject.transform.localPosition = new Vector3(0, 0.2f, 0);
        handObject.transform.localRotation = Quaternion.identity;

        ClearCurrentMainHandObject();

        Debug.Log($"{handObject.name} a été placé dans {recipient.name} avec position gelée et retiré de la main principale.");
    }




    private GameObject GetCurrentMainHandObject()
    {
        return changeMainHand.currentMainHandObject;
    }

    private void ClearCurrentMainHandObject()
    {
        if (changeMainHand.currentMainHandIndex == 0)
        {
            pickupHandler.ClearRightHandObject();
        }
        else
        {
            pickupHandler.ClearLeftHandObject();
        }
    }
}
