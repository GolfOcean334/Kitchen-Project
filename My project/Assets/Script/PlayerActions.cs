using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private DefaultInputActions _defaultInputActions;

    private InputAction _MoveAction;
    private InputAction _LookAction;

    private Rigidbody _Rigidbody;

    private float speed = 6f;
    private float lookSpeed = 0.5f;
    private float pitch = 0f;

    private void Awake()
    {
        _defaultInputActions = new DefaultInputActions();
        _Rigidbody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        _MoveAction = _defaultInputActions.Player.Move;
        _MoveAction.Enable();

        _LookAction = _defaultInputActions.Player.Look;
        _LookAction.Enable();
    }

    private void OnDisable()
    {
        _MoveAction.Disable();
        _LookAction.Disable();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        HandleLook();
    }

    private void HandleMovement()
    {
        Vector2 moveDirection = _MoveAction.ReadValue<Vector2>();
        Vector3 move = transform.forward * moveDirection.y + transform.right * moveDirection.x;
        Vector3 velocity = move * speed;
        velocity.y = _Rigidbody.velocity.y;
        _Rigidbody.velocity = velocity;
    }

    private void HandleLook()
    {
        Vector2 lookDelta = _LookAction.ReadValue<Vector2>();

        float Ylook = lookDelta.x * lookSpeed;
        transform.Rotate(0, Ylook, 0);

        pitch -= lookDelta.y * lookSpeed;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        if (Camera.main != null)
        {
            Camera.main.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }
    }
}
