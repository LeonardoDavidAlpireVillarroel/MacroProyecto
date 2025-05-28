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
    private bool mostrarLevelPanelAutomaticamente = false;
    private bool permitirCerrarMapa = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
<<<<<<< HEAD
        if (ProfileStorage.s_currentProfile == null)
        {
            var profileIndex = ProfileStorage.GetProfileIndex();

            if (profileIndex.ProfileFileNames.Count > 0)
            {
                ProfileStorage.LoadProfile(profileIndex.ProfileFileNames[0]);
            }
        }

        if (UnlockLevelData.sharedUnlockLevel > 0)
        {
            unlockLevel = UnlockLevelData.sharedUnlockLevel;
            if (ProfileStorage.s_currentProfile != null &&
                unlockLevel > ProfileStorage.s_currentProfile.unlockedLevelCount)
            {
                ProfileStorage.s_currentProfile.unlockedLevelCount = unlockLevel;
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    ProfileStorage.StorePlayerProfile(player);
                }
            }
            UnlockLevelData.sharedUnlockLevel = -1;
        }
        else
        {
            unlockLevel = Mathf.Max(ProfileStorage.s_currentProfile.unlockedLevelCount, 2);
        }

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
=======
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentScene == "ClaroPacifico")
        {
            permitirCerrarMapa = true;
        }
        else
        {
            mostrarLevelPanelAutomaticamente = true;
            permitirCerrarMapa = false;
        }

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
>>>>>>> Development
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
            levelPanel.SetActive(false);

        if (interactionText != null)
            interactionText.SetActive(false);

        Time.timeScale = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !mostrarLevelPanelAutomaticamente)
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
        if (other.CompareTag("Player") && !mostrarLevelPanelAutomaticamente)
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
        if (mostrarLevelPanelAutomaticamente) return;

        if (playerInRange && playerController.interactAction.WasPressedThisFrame())
        {
            if (levelPanel != null)
            {
                interactionText.SetActive(false);
                levelPanel.SetActive(true);

                gameManager.PauseGame();
                playerController.enabled = false;
<<<<<<< HEAD

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
=======
>>>>>>> Development
            }
            if (levelPanel != null && levelPanel.activeSelf && gameManager.backAction.WasPressedThisFrame())
            {
                levelPanel.SetActive(false);
                playerController.enabled = true;

<<<<<<< HEAD
=======
        if (levelPanel != null && levelPanel.activeSelf && gameManager.backAction.WasPressedThisFrame())
        {
            if (permitirCerrarMapa)
            {
                levelPanel.SetActive(false);
                playerController.enabled = true;
>>>>>>> Development
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

    public void MostrarPanelMapaAutomaticamente()
    {
        mostrarLevelPanelAutomaticamente = true;

        if (levelPanel != null)
            levelPanel.SetActive(true);

        if (interactionText != null)
            interactionText.SetActive(false);

        if (gameManager != null)
            gameManager.PauseGame();

        if (playerController != null)
            playerController.enabled = false;
    }
}
