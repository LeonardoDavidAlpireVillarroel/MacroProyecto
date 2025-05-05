using UnityEngine;
using UnityEngine.InputSystem;

public class OpenInventory : MonoBehaviour
{
    public GameObject inventoryUIPanel;
    public bool isOpen;
    public PlayerController playerController;

    private void Update()
    {
        if (isOpen)
        {
            playerController.playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            playerController.playerInput.SwitchCurrentActionMap("Player");
        }
    }
}
