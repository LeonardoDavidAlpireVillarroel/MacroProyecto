using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public static MapController Instance;

    public GameObject levelPanel = null;
    public GameObject interactionText = null;

    public Button[] levelButtons;
    public int unlockLevel;

    public PlayerController playerController = null;
    private bool playerInRange = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (levelButtons.Length > 0)
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = false;
            }

            for (int i = 0; i < PlayerPrefs.GetInt("UnlockLevels", 1); i++)
            {
                levelButtons[i].interactable = true;
            }
        }

        if (levelPanel != null)
        {
            levelPanel.SetActive(false);
        }
        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionText != null)
            {
                interactionText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionText != null)
            {
                interactionText.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (playerInRange && playerController.interactAction.WasPressedThisFrame())
        {
            if (levelPanel != null)
            {
                interactionText.SetActive(false);
                levelPanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    public void ClosePanel()
    {
        if (levelPanel != null)
        {
            levelPanel.SetActive(false);
            interactionText.SetActive(true);
            Time.timeScale = 1f;
        }
    }

    public void UnlockLevels()
    {
        if (unlockLevel > PlayerPrefs.GetInt("UnlockLevels", 1))
        {
            PlayerPrefs.SetInt("UnlockLevels", unlockLevel);
        }
    }
}
