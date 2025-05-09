using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Paneles")]
    public GameObject pausePanel;
    public GameObject pauseMenuPanel;
    public GameObject buttonOptionsPanel;
    public GameObject controlsPanel;
    public GameObject optionsPanel;

    public GameObject levelPanel;
    public GameObject interactionText;

    public bool isPaused = false;

    [SerializeField] public PlayerController playerController;

    // HUD Player
    public PlayerHUD playerHUD;
    public int TotalPoints { get; private set; }
    [Header("Player Stats")]
    public int health;
    public int fuerza;
    public int points;

    [Header("Inventario")]
    public Inventory inventory;
    public GameObject inventoryUIPanel;
    public bool isInventoryOpen;

    [Header("Tiendas")]
    public OpenShop shopScript;

    // Input Actions
    [HideInInspector] private PlayerInput playerInput;
    [HideInInspector] public InputAction inventoryAction;
    [HideInInspector] private InputAction pauseAction;
    [HideInInspector] public InputAction backAction;

    // Shoot/Aim Inputs
    [HideInInspector] public InputAction aimAction;
    [HideInInspector] public InputAction shootAction;
    [HideInInspector] public InputAction pointerPositionAction;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadGame();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerInput = playerObject.GetComponent<PlayerInput>();
        }

        if (playerInput != null)
        {
            inventoryAction = playerInput.actions["Inventory"];
            pauseAction = playerInput.actions["Pause"];
            backAction = playerInput.actions["Back"];

            aimAction = playerInput.actions["Aim"];
            shootAction = playerInput.actions["Shoot"];
            pointerPositionAction = playerInput.actions["PointerPosition"];

            inventoryAction.Enable();
            pauseAction.Enable();
            backAction.Enable();
        }
    }

    private void Start()
    {
        if (inventory == null)
        {
            inventory = Inventory.Instance;
        }

        LoadGame();
    }

    void Update()
    {
        if (inventoryAction.WasPressedThisFrame())
        {
            playerController.GetComponent<FruitShoot>().enabled = false;
            isInventoryOpen = true;
            ToggleInventory();
        }

        if (pauseAction.WasPressedThisFrame())
        {
            if (pausePanel != null && pausePanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                isPaused = !isPaused;
                if (pausePanel != null)
                    pausePanel.SetActive(true);
                PauseGame();
            }
        }

        if (backAction.WasPressedThisFrame())
        {
            if (inventory.isInventoryOpen == true)
            {
                ToggleInventory();
            }
            else if (shopScript != null && shopScript.shopCanvasGroup.alpha == 1f)
            {
                shopScript.CloseAllPanels();
                playerController.playerInput.SwitchCurrentActionMap("Player");
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;                
            }
            else if (pausePanel != null && pausePanel.activeSelf)
            {
                ResumeGame();                
            }
        }
    }

    public void SaveGame()
    {
        string profileName = ProfileStorage.s_currentProfile.name;  // O el nombre del perfil actual

        ProfileStorage.s_currentProfile.points = points;
        ProfileStorage.s_currentProfile.fuerza = fuerza;
        ProfileStorage.s_currentProfile.playerHealth = health;

        ProfileStorage.s_currentProfile.inventoryJson = inventory.GetInventoryAsString();

        ProfileStorage.StorePlayerProfile(GameObject.FindWithTag("Player"), this);
    }

    public void LoadGame()
    {
        if (ProfileStorage.s_currentProfile != null)
        {
            points = ProfileStorage.s_currentProfile.points;
            fuerza = ProfileStorage.s_currentProfile.fuerza;
            health = ProfileStorage.s_currentProfile.playerHealth;

            inventory.SetInventoryFromString(ProfileStorage.s_currentProfile.inventoryJson);

            playerHUD.ActualizePoints(points);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        playerController.playerInput.SwitchCurrentActionMap("UI");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerController.GetComponent<FruitShoot>().enabled = false;
        isPaused = true;

        SaveGame();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (levelPanel != null)
            levelPanel.SetActive(false);

        playerController.enabled = true;

        playerController.playerInput.SwitchCurrentActionMap("Player");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerController.GetComponent<FruitShoot>().enabled = true;
        isPaused = false;
    }

    public void SumarPuntos(int pointsToSumar)
    {
        TotalPoints += pointsToSumar;
        playerHUD.ActualizePoints(TotalPoints);
    }

    public void LoseLifes()
    {
        health -= 1;

        if (health >= 0 && health < playerHUD.vidas.Length)
        {
            playerHUD.DesactivateLifes(health);
        }
    }

    public void RecoverLifes()
    {
        playerHUD.ActivateLife(health);
        health += 1;
    }

    private void ToggleInventory()
    {
        if (inventory != null)
        {
            inventory.ToggleInventory();
        }
    }
}