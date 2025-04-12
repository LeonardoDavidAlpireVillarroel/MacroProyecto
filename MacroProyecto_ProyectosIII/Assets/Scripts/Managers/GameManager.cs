using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject pauseMenuPanel;
    public GameObject buttonOptionsPanel;
    public GameObject controlsPanel;
    public GameObject optionsPanel;
    public bool isPaused = false;

    [SerializeField] private PlayerController playerController;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            isPaused = !isPaused;
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused)
        {
            Time.timeScale = 0;

            if (pausePanel != null)
                pausePanel.SetActive(true);

            if (playerController != null)
            {
                if (playerController.playerInput != null)
                    playerController.playerInput.enabled = false;

                if (playerController.cinemachineBrain != null)
                    playerController.cinemachineBrain.enabled = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;

            if (pausePanel != null)
            {
                pauseMenuPanel.SetActive(true);
                buttonOptionsPanel.SetActive(true);
                controlsPanel.SetActive(false);
                optionsPanel.SetActive(false); 
                pausePanel.SetActive(false);
            }

            if (playerController != null)
            {
                if (playerController.playerInput != null)
                    playerController.playerInput.enabled = true;

                if (playerController.cinemachineBrain != null)
                    playerController.cinemachineBrain.enabled = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            isPaused = false;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        pauseMenuPanel.SetActive(true);
        buttonOptionsPanel.SetActive(true);
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        pausePanel.SetActive(false);

        if (playerController != null)
        {
            if (playerController.playerInput != null)
                playerController.playerInput.enabled = true;

            if (playerController.cinemachineBrain != null)
                playerController.cinemachineBrain.enabled = true;
        }

        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
