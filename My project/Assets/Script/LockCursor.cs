using UnityEngine;
using UnityEngine.InputSystem;

public class LockCursor : MonoBehaviour
{
    private GameObject currentTarget;
    private Camera _camera;
    private static bool isCursorUnlocked = false;
    public bool notSyncCursor = true;

    private void Awake()
    {
        _camera = Camera.main;
        LockCursorToCenter();
    }

    private void Update()
    {
        HandleCursorState();
    }

    private void HandleCursorState()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Canvas"))
            {
                if (!isCursorUnlocked)
                {
                    UnlockCursor();
                    Debug.Log("Canvas touché : interaction activée");
                }
                currentTarget = hitObject;
            }
            else if (isCursorUnlocked)
            {
                LockCursorToCenter();
            }
        }
        else if (isCursorUnlocked)
        {
            LockCursorToCenter();
        }
        if (isCursorUnlocked && notSyncCursor == true)
        {
            SyncCrosshairWithCursor();
        }
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        isCursorUnlocked = true;
    }

    private void LockCursorToCenter()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isCursorUnlocked = false;
    }

    private void SyncCrosshairWithCursor()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Vector2 currentCursorPosition = Mouse.current.position.ReadValue();
        Vector2 cursorDelta = (screenCenter - currentCursorPosition) * 0.1f;
        Mouse.current.WarpCursorPosition(screenCenter);
    }
}
