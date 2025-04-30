using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject pauseMenuPanel;
    public GameObject buttonOptionsPanel;
    public GameObject controlsPanel;
    public GameObject optionsPanel;

    public GameObject levelPanel;
    public GameObject interactionText;

    public bool isPaused = false;

    [SerializeField] private PlayerController playerController;

    void Update()
    {
        if (playerController.pauseAction.WasPressedThisFrame())
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

        if (playerController.playerInput.actions["Back"].WasPressedThisFrame() && isPaused)
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        playerController.playerInput.SwitchCurrentActionMap("UI");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (levelPanel != null)
            levelPanel.SetActive(false);

        playerController.playerInput.SwitchCurrentActionMap("Player");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }
}
