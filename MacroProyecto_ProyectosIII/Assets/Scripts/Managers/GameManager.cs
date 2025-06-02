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
    public bool isTutorialScene = false;
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

    [Header("UI Vida Máxima")]
    public CanvasGroup maxLifeMessagePanel;
    public float maxLifeMessageDuration = 2f;
    public float maxLifeFadeDuration = 1f;

    [Header("Respawn")]
    public Transform respawnPoint;

    [Header("Player Stats")]
    public int health;
    public int fuerza;
    public int points;

    private int tempHealth;
    private int tempPoints;
    private int tempFuerza;
    private string tempInventoryJson;

    private int lastSavedPoints = -1;
    private int lastSavedHealth = -1;
    private string lastSavedInventory = "";

    [Header("Pantalla Game Over")]
    public GameObject gameOverPanel;

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
    [HideInInspector] public InputAction meleeAction;
    [HideInInspector] public InputAction pointerPositionAction;

    [Header("UI de Avisos")]
    public CanvasGroup warningShootPanel;
    public float warningDuration = 2f;
    public float fadeDuration = 0.5f;

    [Header("Levels")]
    public int currentItemsCollected = 0;
    public int currentEnemiesDefeated = 0;
    public int tiempoLimite = 0;

    [Header("Controlador niveles")]
    public LevelController levelTimer;


    public float invulnerableTime = 1.0f;

    //Banderas Bool
    public bool isInvulnerable = false;
    private bool isReturningToClaro = false;
    private bool isProcessingGameOver = false;
    private bool hasDiedAndNeedsLifeReset = false;

    private bool CheckIfPlayerDataChanged()
    {
        bool changed = false;

        if (TotalPoints != lastSavedPoints || health != lastSavedHealth || inventory.GetInventoryAsString() != lastSavedInventory)
        {
            changed = true;
            lastSavedPoints = TotalPoints;
            lastSavedHealth = health;
            lastSavedInventory = inventory.GetInventoryAsString();
        }

        return changed;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    
    public void BackupCurrentState()
    {
        tempHealth = health;
        tempPoints = points;
        tempFuerza = fuerza;
        tempInventoryJson = inventory.GetInventoryAsString();
    }

    public void RestoreBackupState()
    {
        health = tempHealth;
        points = tempPoints;
        fuerza = tempFuerza;

        playerHUD.ActualizePoints(points);
        playerHUD.UpdateAllLifes(health);
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
            meleeAction = playerInput.actions["Melee"];
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
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "ClaroPacifico")
        {
            if (CheckIfPlayerDataChanged())
            {
                SaveGame();
            }
        }

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

        if (SceneManager.GetActiveScene().name == "ClaroPacifico" && aimAction.WasPressedThisFrame()
            && isPaused == false && Inventory.Instance.isInventoryOpen == false
            && (shopScript != null && (shopScript.shopCanvasGroup.alpha == 0)))
        {
            if (warningShootPanel != null)
            {
                StartCoroutine(ShowShootWarningCoroutine());
            }
        }
    }

    public void SaveGame()
    {
        if (isTutorialScene)
        {
            return;
        }

        if (ProfileStorage.s_currentProfile == null) return;

        ProfileStorage.s_currentProfile.points = TotalPoints;
        ProfileStorage.s_currentProfile.fuerza = fuerza;
        ProfileStorage.s_currentProfile.playerHealth = health;
        ProfileStorage.s_currentProfile.inventoryJson = inventory.GetInventoryAsString();

        ProfileStorage.StorePlayerProfile(GameObject.FindWithTag("Player"), this);
    }

    public void LoadGame()
    {
        if (ProfileStorage.s_currentProfile != null)
        {
            health = ProfileStorage.s_currentProfile.playerHealth;

            if (health <= 0)
            {
                health = 3;
            }

            fuerza = ProfileStorage.s_currentProfile.fuerza;
            TotalPoints = ProfileStorage.s_currentProfile.points;
            points = TotalPoints;

            Inventory.Instance.SetInventoryFromString(ProfileStorage.s_currentProfile.inventoryJson);

            Vector2 playerPos = new Vector2(ProfileStorage.s_currentProfile.x, ProfileStorage.s_currentProfile.y);
            transform.position = playerPos;

            playerHUD.UpdateAllLifes(health);
        }

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
                }
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (sceneName == "ClaroPacifico")
        {
            if (hasDiedAndNeedsLifeReset)
            {
                health = 3;
                playerHUD.UpdateAllLifes(health);

                if (ProfileStorage.s_currentProfile != null)
                {
                    ProfileStorage.s_currentProfile.playerHealth = health;
                }

                SaveGame();

                hasDiedAndNeedsLifeReset = false;
            }
            else
            {
                LoadGame();
            }
        }
        else
        {
            BackupCurrentState();
            if (levelTimer != null)
            {
                levelTimer.BackupCollectedItems();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        playerController.playerInput.actions.FindActionMap("UI").Enable();

        isPaused = true;
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

        isPaused = false;
    }

    public void SumarPuntos(int pointsToSumar)
    {
        TotalPoints += pointsToSumar;
        points = TotalPoints;
        playerHUD.ActualizePoints(TotalPoints);
    }

    public void UsarPocionPorID(int id)
    {
        var itemData = inventory.data.ObjectsDataBase[id];

        if (itemData.clase != ItemsDataBase.Clase.Pocion || itemData.type != ItemsDataBase.Type.consumable)
        {
            return;
        }

        switch (id)
        {
            case 1:
                int maxHealth = playerHUD.vidas.Length;

                if (health < maxHealth)
                {
                    health += 1;
                    playerHUD.ActivateLife(health - 1);

                    RemoveOnePotionFromInventory(id);
                }
                else
                {
                    StartCoroutine(ShowMaxLifeMessage());
                }
                break;

            case 2:
                fuerza += 1;
                RemoveOnePotionFromInventory(id);
                break;

            default:
                Debug.Log($"No hay efecto definido para la poción con ID: {id}");
                break;
        }

        SaveGame();
    }

    private void RemoveOnePotionFromInventory(int id)
    {
        for (int i = 0; i < inventory.inventory.Count; i++)
        {
            if (inventory.inventory[i].id == id && inventory.inventory[i].cantidadItems > 0)
            {
                int newAmount = inventory.inventory[i].cantidadItems - 1;

                if (newAmount <= 0)
                {
                    inventory.inventory[i] = new ObjectInventoryID(-1, 0);
                }
                else
                {
                    inventory.inventory[i] = new ObjectInventoryID(id, newAmount);
                }

                inventory.InventoryUpdate();
                break;
            }
        }
    }

    public IEnumerator ShowMaxLifeMessage()
    {
        if (maxLifeMessagePanel == null) yield break;

        maxLifeMessagePanel.alpha = 1f;
        maxLifeMessagePanel.gameObject.SetActive(true);

        yield return new WaitForSeconds(maxLifeMessageDuration);

        float elapsed = 0f;
        while (elapsed < maxLifeFadeDuration)
        {
            elapsed += Time.deltaTime;
            maxLifeMessagePanel.alpha = Mathf.Lerp(1f, 0f, elapsed / maxLifeFadeDuration);
            yield return null;
        }

        maxLifeMessagePanel.alpha = 0f;
        maxLifeMessagePanel.gameObject.SetActive(false);
    }

    public void LoseLifes()
    {
        if (isInvulnerable || isProcessingGameOver) return;

        if (health > 0)
        {
            health -= 1;
            playerHUD.DesactivateLifes(health);

            SaveGame();

            StartCoroutine(InvulnerabilityCoroutine());

            if (health == 0)
            {
                TriggerGameOver();
                if (playerController != null)
                    playerController.enabled = false;
            }
        }
    }

    public void RecoverLifes()
    {
        playerHUD.ActivateLife(health);
        health += 1;
    }


    public void TriggerGameOver()
    {
        if (isProcessingGameOver) return;

        isProcessingGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            PauseGame();
        }

        if (levelTimer != null)
        {
            levelTimer.EliminarItemsRecolectadosDelInventario();
            levelTimer.ClearCollectedItems();
        }

        SaveGame();

        RestoreBackupState();

        hasDiedAndNeedsLifeReset = true;

        StartCoroutine(ReturnToClaroAfterDelay(2f));
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerableTime);
        isInvulnerable = false;
    }


    private IEnumerator ReturnToClaroAfterDelay(float delay)
    {
        if (isReturningToClaro) yield break;
        isReturningToClaro = true;

        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 1f;

        MusicManager.Instance.PlayMusic("ClaroPacifico");
        ScenesManager.Instance.LoadScene("ClaroPacifico", "CrossFade");

        isReturningToClaro = false;
    }

    public void OnGameOverConfirm()
    {
        gameOverPanel.SetActive(false);
        StartCoroutine(ReturnToClaroAfterDelay(0f));
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

    public void AddFruit()
    {
        currentItemsCollected++;
        CheckAndFinishLevelIfObjectivesCompleted();
    }
    public void EnemyDefeated()
    {
        currentEnemiesDefeated++;
        CheckAndFinishLevelIfObjectivesCompleted();
    }

    public void CheckAndFinishLevelIfObjectivesCompleted()
    {
        if (levelTimer != null && levelTimer.IsTimerActive() && levelTimer.AreAllObjectivesCompleted())
        {
            levelTimer.FinishLevelEarly("¡Objetivos completados!");
        }
    }

    private bool levelCompleted = false;

    public void OnLevelCompleted()
    {
        if (levelCompleted) return;

        levelCompleted = true;

        points = TotalPoints;
        playerHUD.ActualizePoints(TotalPoints);

        var levelName = SceneManager.GetActiveScene().name;
        var stats = ProfileStorage.s_currentProfile.GetLevelStats(levelName);

        float tiempoRestante = levelTimer != null ? levelTimer.ObtenerTiempoRestante() : 0f;
        stats.timeCompleted = tiempoLimite - tiempoRestante;
        stats.fruitsCollected = currentItemsCollected;
        stats.enemiesDefeated = currentEnemiesDefeated;

        ProfileStorage.StorePlayerProfile(GameObject.FindWithTag("Player"), this);

        SaveGame();
    }
}