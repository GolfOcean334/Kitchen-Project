using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenContainerHandler : MonoBehaviour
{
    private GameObject _currentTarget;
    [SerializeField] private Camera _camera;

    public GameObject containerPanel;

    private PlayerInput playerInput;
    private InputAction openContainer;
    private InputAction closeContainer;

    [SerializeField] private PlayerActions playerActions;

    private void Awake()
    {
        playerInput = new PlayerInput();
        openContainer = playerInput.Player.ContainerOpen;
        closeContainer = playerInput.Player.CloseUI;
        _camera = Camera.main;
    }
    private void OnEnable()
    {
        openContainer.Enable();
        closeContainer.Enable();
    }
    private void OnDisable()
    {
        openContainer.Disable();
        closeContainer.Disable();
    }
    private void Start()
    {
        containerPanel.SetActive(false);
    }
    private void Update()
    {
        DetectionContainer();
        if (openContainer.triggered)
        {
            OpenContainer();
            playerActions.StopPlayerMovement();
        }
        if (closeContainer.triggered)
        {
            CloseUIContainer();
        }
    }
    private void DetectionContainer()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Container"))
            {
                if (_currentTarget != hitObject)
                {
                    StopDetectionContainer();
                    _currentTarget = hitObject;
                }
            }
            else
            {
                StopDetectionContainer();
            }
        }
        else
        {
            StopDetectionContainer();
        }
    }
    public void StopDetectionContainer()
    {
        if (_currentTarget != null)
        {
            _currentTarget = null;
        }
    }

    private void OpenContainer()
    {
        if (_currentTarget != null)
        {
            if (playerActions != null)
            {
                playerActions.enabled = false;
            }
            ShowUIContainer();
        }
    }
    private void ShowUIContainer()
    {
        containerPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CloseUIContainer()
    {
        containerPanel.SetActive(false);
        if (playerActions != null)
        {
            playerActions.enabled = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
