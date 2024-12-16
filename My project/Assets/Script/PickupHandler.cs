using UnityEngine;
using TMPro;

public class PickupHandler : MonoBehaviour
{
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;

    [SerializeField] private GameObject imgRightHand;
    [SerializeField] private GameObject imgLeftHand;

    [SerializeField] private Camera playerCamera;
    private GameObject rightHandObject;
    private GameObject leftHandObject;

    [SerializeField] private HighlightHandler highlightHandler;

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
            imgRightHand.SetActive(false);
        }
        else if (leftHandObject == null)
        {
            AttachObjectToHand(obj, leftHand, ref leftHandObject);
            imgLeftHand.SetActive(false);
        }
        else
        {
            Debug.Log("Les deux mains sont déjà occupées !");
        }
    }

    private void AttachObjectToHand(GameObject obj, Transform hand, ref GameObject handObject)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Disable colliders
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
}