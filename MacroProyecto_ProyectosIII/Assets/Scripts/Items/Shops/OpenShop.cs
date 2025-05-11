using UnityEngine;
using UnityEngine.UI;

public class OpenShop : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject proximityPanel;
    public GameObject lockedPanel;

    [Header("Canvas Groups")]
    public CanvasGroup shopCanvasGroup;

    [Header("Settings")]
    public bool storeLocked = false;

    private bool isPlayerInside = false;
    public PlayerController playerController;

    public GameManager gameManager;

    private void Start()
    {
        proximityPanel.SetActive(false);
        lockedPanel.SetActive(false);
        DisableCanvasGroup(shopCanvasGroup);

        GameObject playerObject = GameObject.FindWithTag("Player");
        playerController = playerObject.GetComponent<PlayerController>();

        gameManager = GameManager.Instance;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            proximityPanel.SetActive(true);
            lockedPanel.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            proximityPanel.SetActive(false);
            lockedPanel.SetActive(false);
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

    private void OpenInteractionPanel()
    {
        gameManager.playerController.playerInput.actions.FindActionMap("UI").Enable();
        playerController.moveAction.Disable();
        proximityPanel.SetActive(false);
        lockedPanel.SetActive(false);
        EnableCanvasGroup(shopCanvasGroup);

        if (gameManager.isInventoryOpen)
        {
            gameManager.inventory.ToggleInventory();
        }
    }

    private void EnableCanvasGroup(CanvasGroup cg)
    {
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        playerController.GetComponent<FruitShoot>().enabled = false;
    }

    private void DisableCanvasGroup(CanvasGroup cg)
    {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        playerController.GetComponent<FruitShoot>().enabled = true;
        playerController.moveAction.Enable();
    }

    private void OpenLockedPanel()
    {
        proximityPanel.SetActive(false);
        DisableCanvasGroup(shopCanvasGroup);
        lockedPanel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        gameManager.playerController.playerInput.actions.FindActionMap("UI").Disable();
        lockedPanel.SetActive(false);
        proximityPanel.SetActive(false);
        DisableCanvasGroup(shopCanvasGroup);
    }
}
