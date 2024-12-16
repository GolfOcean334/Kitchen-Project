using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMainHand : MonoBehaviour
{
    [SerializeField] private PickupHandler pickupHandler;
    [SerializeField] private HighlightHandler highlightHandler;

    private int currentMainHandIndex = 0;
    private GameObject currentMainHandObject;

    private void Update()
    {
        float scrollValue = Input.mouseScrollDelta.y;

        if (scrollValue > 0)
        {
            SetMainHand(0);
        }
        else if (scrollValue < 0)
        {
            SetMainHand(1);
        }
    }

    private void SetMainHand(int handIndex)
    {
        if (currentMainHandIndex == handIndex)
            return;

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
