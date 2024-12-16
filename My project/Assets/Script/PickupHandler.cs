using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    public Transform rightHand;
    public Transform leftHand;

    public Camera playerCamera;
    private GameObject rightHandObject;
    private GameObject leftHandObject;

    public HighlightHandler highlightHandler;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupObject();
        }

        UpdateHandPositions();
    }

    private void TryPickupObject()
    {
        GameObject target = highlightHandler.GetCurrentTarget();
        if (target != null)
        {
            PickupObject(target);
            highlightHandler.ClearHighlight();
        }
    }

    private void PickupObject(GameObject obj)
    {
        if (rightHandObject == null)
        {
            AttachObjectToHand(obj, rightHand, ref rightHandObject);
        }
        else if (leftHandObject == null)
        {
            AttachObjectToHand(obj, leftHand, ref leftHandObject);
        }
        else
        {
            Debug.Log("Les deux mains sont déjà occupées !");
        }
    }

    private void AttachObjectToHand(GameObject obj, Transform hand, ref GameObject handObject)
    {
        // Disable physics (kinematic)
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Disable colliders
        Collider[] colliders = obj.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        // Attach object to the hand
        obj.transform.SetParent(hand);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        handObject = obj;

        // Remove the tag to prevent other interactions
        obj.tag = "Untagged";
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
}
