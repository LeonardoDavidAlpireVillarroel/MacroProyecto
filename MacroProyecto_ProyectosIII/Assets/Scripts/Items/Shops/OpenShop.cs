using UnityEngine;

public class OpenShop : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject proximityPanel;
    public GameObject shopPanel;
    public GameObject lockedPanel;

    [Header("Settings")]
    public bool storeLocked = false;

    private bool isPlayerInside = false;
    public PlayerController playerController;

    private void Start()
    {
        proximityPanel.SetActive(false);
        shopPanel.SetActive(false);
        lockedPanel.SetActive(false);

        GameObject playerObject = GameObject.FindWithTag("Player");
        playerController = playerObject.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            proximityPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            OnExitPanel();
        }
    }

    private void Update()
    {
        if (isPlayerInside && playerController.interactAction.WasPressedThisFrame())
        {
            OnInteract();
        }
    }

    private void OnInteract()
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

    private void OnExitPanel()
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
        proximityPanel.SetActive(false);
        lockedPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseAllPanels()
    {
        shopPanel.SetActive(false);
        lockedPanel.SetActive(false);
        proximityPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
