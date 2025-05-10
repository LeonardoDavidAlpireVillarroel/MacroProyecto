using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("UI de Avisos")]
    public CanvasGroup warningShootPanel;
    public float warningDuration = 2f;
    public float fadeDuration = 0.5f;


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
            }
            else if (pausePanel != null && pausePanel.activeSelf)
            {
                ResumeGame();                
            }
        }

        if (SceneManager.GetActiveScene().name == "ClaroPacifico" && (shootAction.WasPressedThisFrame() || aimAction.WasPressedThisFrame()))
        {
            if (warningShootPanel != null)
            {
                StartCoroutine(ShowShootWarningCoroutine());
            }
        }
    }

    public void SaveGame()
    {
        string profileName = ProfileStorage.s_currentProfile.name;

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

        playerHUD.UpdateAllLifes(health);

        string currentScene = SceneManager.GetActiveScene().name;

        if (playerController != null)
        {
            FruitShoot fruitShoot = playerController.GetComponent<FruitShoot>();
            var playerInput = playerController.playerInput;

            if (fruitShoot != null && playerInput != null)
            {
                if (currentScene == "ClaroPacifico")
                {
                    fruitShoot.enabled = false;
                }
                else
                {
                    fruitShoot.enabled = true;

                    playerInput.actions["Shoot"].Enable();
                    playerInput.actions["Aim"].Enable();
                }
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        playerController.playerInput.actions.FindActionMap("UI").Enable();
        playerController.playerInput.actions.FindActionMap("Player").Disable();

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

        playerController.playerInput.actions.FindActionMap("UI").Disable();
        playerController.playerInput.actions.FindActionMap("Player").Enable();

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

    public void UpdateCursorState()
    {
        if (Inventory.Instance.isInventoryOpen || isPaused || (shopScript != null && shopScript.shopCanvasGroup.alpha > 0f))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (playerController.GetComponent<FruitShoot>().isAiming)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private IEnumerator ShowShootWarningCoroutine()
    {
        if (warningShootPanel == null) yield break;

        warningShootPanel.gameObject.SetActive(true);

        CanvasGroup cg = warningShootPanel.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = warningShootPanel.gameObject.AddComponent<CanvasGroup>();
        }

        cg.alpha = 1f;

        yield return new WaitForSeconds(warningDuration);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = 0f;
        warningShootPanel.gameObject.SetActive(false);
    }
}