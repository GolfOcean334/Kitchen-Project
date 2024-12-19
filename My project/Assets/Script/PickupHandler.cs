using UnityEngine;
using UnityEngine.InputSystem;

public class PickupHandler : MonoBehaviour
{
    [SerializeField] public Transform rightHand;
    [SerializeField] public Transform leftHand;

    [SerializeField] public GameObject imgRightHand;
    [SerializeField] public GameObject imgLeftHand;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private ChangeMainHand changeMainHand;
    public GameObject rightHandObject;
    public GameObject leftHandObject;

    [SerializeField] private HighlightHandler highlightHandler;

    private PlayerInput playerInput;
    private InputAction pickupAction;

    private void Awake()
    {
        playerInput = new PlayerInput();
        pickupAction = playerInput.Player.Pickup;
    }
    private void OnEnable()
    {
        pickupAction.Enable();
        pickupAction.performed += OnPickupPerformed;
    }

    private void OnDisable()
    {
        pickupAction.Disable();
        pickupAction.performed -= OnPickupPerformed;
    }
    private void OnPickupPerformed(InputAction.CallbackContext context)
    {
        TryPickupObject();
    }
    private void Update()
    {
        UpdateHandPositions();
    }

    private void TryPickupObject()
    {
        GameObject target = highlightHandler.GetCurrentTarget();
        if (target != null)
        {
            PickupObject(target);

            Rigidbody rb = target.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            highlightHandler.RemoveHighlight(target);
        }
    }

    private void PickupObject(GameObject obj)
    {
        if (rightHandObject == null)
        {
            AttachObjectToHand(obj, rightHand, ref rightHandObject);
            imgRightHand.SetActive(false);
            changeMainHand.SetMainHand(0);
        }
        else if (leftHandObject == null)
        {
            AttachObjectToHand(obj, leftHand, ref leftHandObject);
            imgLeftHand.SetActive(false);
            changeMainHand.SetMainHand(1);
        }
        else
        {
            Debug.Log("Les deux mains sont déjà occupées !");
        }
    }

    public void AttachObjectToHand(GameObject obj, Transform hand, ref GameObject handObject)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider[] colliders = obj.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        obj.transform.SetParent(hand);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        handObject = obj;
    }

    private float verticalOffset = -0.3f;

    private void UpdateHandPositions()
    {
        rightHand.position = playerCamera.transform.position + playerCamera.transform.right * 0.5f
                             + playerCamera.transform.forward * 0.6f
                             + playerCamera.transform.up * verticalOffset;
        rightHand.rotation = playerCamera.transform.rotation;

        leftHand.position = playerCamera.transform.position - playerCamera.transform.right * 0.5f
                            + playerCamera.transform.forward * 0.6f
                            + playerCamera.transform.up * verticalOffset;
        leftHand.rotation = playerCamera.transform.rotation;
    }

    public GameObject GetRightHandObject()
    {
        return rightHandObject;
    }

    public GameObject GetLeftHandObject()
    {
        return leftHandObject;
    }

    public void ClearRightHandObject()
    {
        if (rightHandObject != null)
        {
            DetachObject(rightHandObject);
            rightHandObject = null;
            imgRightHand.SetActive(true);
        }
    }

    public void ClearLeftHandObject()
    {
        if (leftHandObject != null)
        {
            DetachObject(leftHandObject);
            leftHandObject = null;
            imgLeftHand.SetActive(true);
        }
    }
    public GameObject GetCurrentMainHandObject()
    {
        return changeMainHand.currentMainHandObject;
    }
    public void ClearCurrentMainHandObject()
    {
        if (changeMainHand.currentMainHandIndex == 0)
        {
            ClearRightHandObject();
        }
        else
        {
            ClearLeftHandObject();
        }
    }

    private void DetachObject(GameObject obj)
    {
        obj.transform.SetParent(null);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        Collider[] colliders = obj.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
    }
}
