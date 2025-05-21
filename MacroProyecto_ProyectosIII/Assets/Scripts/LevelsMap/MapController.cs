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

    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;

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
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentScene != "ClaroPacifico") return;

        if (ProfileStorage.s_currentProfile == null)
        {
            string savedProfile = PlayerPrefs.GetString("CurrentProfile", null);
            if (!string.IsNullOrEmpty(savedProfile))
            {
                ProfileStorage.LoadProfile(savedProfile, gameManager);
            }
        }

        unlockLevel = ProfileStorage.s_currentProfile != null ? ProfileStorage.s_currentProfile.unlockedLevelCount : 1;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = i < unlockLevel;
        }

        if (levelButtons.Length > 0)
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = false;
            }

            for (int i = 0; i < unlockLevel && i < levelButtons.Length; i++)
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

                gameManager.PauseGame();
                playerController.enabled = false;
            }
            if (levelPanel != null && levelPanel.activeSelf && gameManager.backAction.WasPressedThisFrame())
            {
                levelPanel.SetActive(false);
                playerController.enabled = true;

                gameManager.ResumeGame();
            }
        }
    }

    public void UnlockLevels()
    {
        UnlockLevel(1);
    }

    public void UnlockLevel(int levelToUnlock)
    {
        if (ProfileStorage.s_currentProfile == null)
            return;

        if (ProfileStorage.s_currentProfile.unlockedLevelCount >= levelToUnlock)
            return;

        ProfileStorage.s_currentProfile.unlockedLevelCount = levelToUnlock;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && GameManager.Instance != null)
        {
            ProfileStorage.StorePlayerProfile(player, GameManager.Instance);
        }
    }
}
