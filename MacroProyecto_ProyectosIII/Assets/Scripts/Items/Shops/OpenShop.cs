using UnityEngine;
using UnityEngine.InputSystem;

public class OpenShop : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject proximityPanel; 
    public GameObject shopPanel;
    public GameObject lockedPanel;

    [Header("Settings")]
    public bool storeLocked = false;

    private bool isPlayerInside = false;
    private PlayerInput playerInput;

    private void Start()
    {
        proximityPanel.SetActive(false);
        shopPanel.SetActive(false);
        lockedPanel.SetActive(false);

        playerInput = FindFirstObjectByType<PlayerInput>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            proximityPanel.SetActive(true);

            playerInput.actions["Interact"].performed += OnInteract;
            playerInput.actions["Back"].performed += OnExitPanel;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            proximityPanel.SetActive(false);
            CloseAllPanels();

            playerInput.actions["Interact"].performed -= OnInteract;
            playerInput.actions["Back"].performed -= OnExitPanel;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!isPlayerInside) return;

        if (storeLocked)
        {
            OpenLockedPanel();
        }
        else
        {
            OpenInteractionPanel();
        }
    }

    private void OnExitPanel(InputAction.CallbackContext context)
    {
        if (shopPanel.activeSelf || lockedPanel.activeSelf)
        {
            CloseAllPanels();
        }
    }

    private void OpenInteractionPanel()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OpenLockedPanel()
    {
        lockedPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseAllPanels()
    {
        shopPanel.SetActive(false);
        lockedPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
