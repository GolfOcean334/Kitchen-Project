using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenBindingPanel : MonoBehaviour
{
    public GameObject bindingPanel;

    private PlayerInput playerInput;
    private InputAction openBindingUI;
    private InputAction closeUI;

    [SerializeField] private PlayerActions playerActions;
    private void Awake()
    {
        playerInput = new PlayerInput();
        openBindingUI = playerInput.Player.OpenBindingUI;
        closeUI = playerInput.Player.CloseUI;
    }
    private void OnEnable()
    {
        openBindingUI.Enable();
        closeUI.Enable();
    }
    private void OnDisable()
    {
        openBindingUI.Disable();
        closeUI.Disable();
    }
    private void Start()
    {
        ClosePanel();
    }
    private void Update()
    {
        if (openBindingUI.triggered)
        {
            OpenPanel();
            playerActions.StopPlayerMovement();
        }
        if (closeUI.triggered)
        {
            ClosePanel();
        }
    }
    public void OpenPanel()
    {
        bindingPanel.SetActive(true);
        if (playerActions != null)
        {
            playerActions.enabled = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ClosePanel()
    {
        bindingPanel.SetActive(false);
        if (playerActions != null)
        {
            playerActions.enabled = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
