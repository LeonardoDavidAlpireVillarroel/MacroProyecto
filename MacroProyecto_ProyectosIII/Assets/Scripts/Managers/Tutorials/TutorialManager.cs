using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum TipoAtaque
{
    Ninguno,
    Melee,
    Disparo,
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public CanvasGroup[] tutorialPanels;
    public int currentStep = -1;

    private PlayerController player;
    private PlayerInput input;

    private bool[] moveKeys = new bool[4]; // W, A, S, D
    private bool jumpDone = false;
    private bool doubleJumpDone = false;
    private bool aimDone = false;

    public GameObject meleeEnemy;
    public GameObject shootEnemy;

    private HashSet<string> enabledInputs = new HashSet<string>();
    public bool panelShowing = false;

    public ItemsDataBase itemsDatabase;

    private bool canBuyHealthPotion = false;
    private bool healthPotionBoughtAfterSell = false;
    private bool healthPotionUsed = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        player = PlayerController.Instance;
        input = player.playerInput;

        if (meleeEnemy != null) meleeEnemy.SetActive(false);
        if (shootEnemy != null) shootEnemy.SetActive(false);

        // Inicia mostrando el panel de introducción (paso 0)
        ShowPanel(0);
    }

    void Update()
    {
        if (currentStep < 0) return;

        switch (currentStep)
        {
            case 1: CheckMovement(); break;
            case 2: CheckJump(); break;
            case 3: CheckDoubleJump(); break;
            case 4: CheckAim(); break;
        }
    }

    #region Comprobaciones de cada paso

    void CheckMovement()
    {
        Vector2 move = player.moveInput;

        if (move.y > 0) moveKeys[0] = true; // W
        if (move.x < 0) moveKeys[1] = true; // A
        if (move.y < 0) moveKeys[2] = true; // S
        if (move.x > 0) moveKeys[3] = true; // D

        if (moveKeys[0] && moveKeys[1] && moveKeys[2] && moveKeys[3])
        {
            currentStep = -1;
            ShowPanel(2); // Panel de salto
        }
    }

    void CheckJump()
    {
        if (!jumpDone && player.capibaraAnimator.GetBool("IsJump"))
        {
            jumpDone = true;
            currentStep = -1;
            ShowPanel(3); // Panel de doble salto
        }
    }

    void CheckDoubleJump()
    {
        if (!doubleJumpDone && player.capibaraAnimator.GetBool("DoubleJump"))
        {
            doubleJumpDone = true;
            currentStep = -1;
            ShowPanel(4); // Panel de apuntado
        }
    }

    float aimHoldTime = 0f;
    void CheckAim()
    {
        if (player.aimAction.ReadValue<float>() > 0.5f)
        {
            aimHoldTime += Time.deltaTime;
            if (!aimDone && aimHoldTime > 0.2f)
            {
                aimDone = true;
                currentStep = -1;
                ShowPanel(5); // Panel de cuerpo a cuerpo
            }
        }
        else
        {
            aimHoldTime = 0f;
        }
    }

    #endregion

    #region Paneles

    public void ShowPanel(int panelIndex)
    {
        if (panelShowing || panelIndex >= tutorialPanels.Length) return;
        panelShowing = true;

        StopAllCoroutines();
        StartCoroutine(ShowPanelWithDelay(panelIndex, 0.8f));
    }

    IEnumerator ShowPanelWithDelay(int panelIndex, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        yield return StartCoroutine(FadeAllPanelsOut());
        StartCoroutine(FadeCanvas(tutorialPanels[panelIndex], 1f, 0.3f));

        GameManager.Instance.PauseGame();
    }

    public void ClosePanelAndContinue(int nextStep)
    {
        StartCoroutine(FadeAllPanelsOut());
        GameManager.Instance.ResumeGame();
        panelShowing = false;

        if (nextStep == 10)  // Si cierras panel 9
        {
            panel9Closed = true;
        }

        switch (nextStep)
        {
            case 1:
                ShowPanel(1); // Panel de movimiento
                break;
            case 2:
                SetStep(1); // Habilita movimiento
                break;
            case 3:
                SetStep(2); // Habilita salto
                break;
            case 4:
                SetStep(3); // Habilita doble salto
                break;
            case 5:
                SetStep(4); // Habilita apuntado
                break;
            case 6: // cerrar panel cuerpo a cuerpo
                SetStep(5); // habilita cuerpo a cuerpo
                meleeEnemy?.SetActive(true);
                break;
            case 7: // cerrar panel disparo
                SetStep(6); // habilita disparo
                shootEnemy?.SetActive(true);
                break;
            case 8:
                SetStep(9); // Ya habilita el inventario y todo lo necesario
                break;
            case 9: // Cierra panel de tienda
                SetStep(8); // Habilita compra
                break;
            case 10: // Cierra panel de vender
                SetStep(9);
                break;
        }
    }

    IEnumerator FadeAllPanelsOut()
    {
        foreach (var panel in tutorialPanels)
        {
            if (panel != null)
                StartCoroutine(FadeCanvas(panel, 0f, 0.3f));
        }
        yield return new WaitForSeconds(0.3f);
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float targetAlpha, float duration)
    {
        float startAlpha = cg.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        cg.alpha = targetAlpha;
        cg.blocksRaycasts = targetAlpha > 0;
        cg.interactable = targetAlpha > 0;
    }

    #endregion

    #region Progresión tutorial y control de inputs

    void SetStep(int step)
    {
        currentStep = step;
        DisableAllInputs();
        enabledInputs.Clear();

        switch (step)
        {
            case 1:
                EnableInput("Move");
                break;
            case 2:
                EnableInput("Move");
                EnableInput("Jump");
                break;
            case 3:
                EnableInput("Move");
                EnableInput("Jump");
                break;
            case 4:
                EnableInput("Move");
                EnableInput("Jump");
                EnableInput("Aim");
                break;
            case 5:
                EnableInput("Move");
                EnableInput("Jump");
                EnableInput("Aim");
                EnableInput("Melee");
                break;
            case 6:
                EnableInput("Move");
                EnableInput("Jump");
                EnableInput("Aim");
                EnableInput("Melee");
                EnableInput("Shoot");
                break;
            case 7:
                EnableInput("Move");
                EnableInput("Jump");
                EnableInput("Aim");
                EnableInput("Melee");
                EnableInput("Shoot");
                EnableInput("Interact");
                break;
            case >=9:
                EnableInput("Move");
                EnableInput("Jump");
                EnableInput("Aim");
                EnableInput("Melee");
                EnableInput("Shoot");
                EnableInput("Interact");
                EnableInput("Inventory"); // Solo se activa tras comprar un ítem y cerrar panel 9
                break;
        }
    }

    void EnableInput(string actionName)
    {
        input.actions[actionName].Enable();
        enabledInputs.Add(actionName);
    }

    void DisableAllInputs()
    {
        foreach (var action in input.actions)
            action.Disable();
    }

    #endregion

    #region Enemigos y otros eventos

    private bool meleeKillTriggered = false;
    private bool shootKillTriggered = false;
    public void OnEnemyKilled(TipoAtaque tipo)
    {
        switch (tipo)
        {
            case TipoAtaque.Melee:
                if (!meleeKillTriggered)
                {
                    meleeKillTriggered = true;
                    currentStep = -1;
                    ShowPanel(6); // Panel de disparo
                }
                break;

            case TipoAtaque.Disparo:
                if (!shootKillTriggered)
                {
                    shootKillTriggered = true;
                    currentStep = -1;
                    ShowPanel(7); // Panel de interacción
                }
                break;
        }
    }

    public void OnShopOpened()
    {
        currentStep = -1;
        ShowPanel(8); // Panel comprar en tienda
    }

    private bool itemBought = false;
    private bool panel9Closed = false;

    public void OnItemBought()
    {
        if (itemBought) return;
        itemBought = true;
        panel9Closed = false;  // Reset al comprar

        currentStep = -1;
        ShowPanel(9);
    }

    public void OnPotionBought(int itemId)
    {
        if (!canBuyHealthPotion || healthPotionBoughtAfterSell) return;

        var item = itemsDatabase.GetItemByID(itemId);
        if (item.HasValue && item.Value.ID == 1)
        {
            healthPotionBoughtAfterSell = true;
        }
    }

    private bool itemDeleted = false;

    public void OnItemDeleted()
    {
        if (itemDeleted || !itemBought || !panel9Closed) return;

        itemDeleted = true;
        canBuyHealthPotion = true;

        currentStep = -1;
        ShowPanel(10);
    }

    public void OnItemUsed(int itemId)
    {
        if (!healthPotionBoughtAfterSell || healthPotionUsed) return;

        var item = itemsDatabase.GetItemByID(itemId);
        if (item.HasValue && item.Value.ID == 1)
        {
            healthPotionUsed = true;
            currentStep = -1;
            ShowPanel(11); // Panel final
        }
    }


    #endregion
}
