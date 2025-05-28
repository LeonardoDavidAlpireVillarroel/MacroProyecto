using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DashTutorialController : MonoBehaviour
{
    public GameObject dashTutorialPanel;
    public Button closeButton;

    void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level2")
        {
            int dashTutorialSeen = ProfileStorage.s_currentProfile.GetPrefInt("dashTutorialSeen", 0);

            if (dashTutorialSeen == 0)
            {
                ShowDashTutorial();
            }
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseDashTutorial);
        }
    }

    public void ShowDashTutorial()
    {
        if (dashTutorialPanel != null)
        {
            dashTutorialPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void CloseDashTutorial()
    {
        if (dashTutorialPanel != null)
        {
            dashTutorialPanel.SetActive(false);
            Time.timeScale = 1f;

            ProfileStorage.s_currentProfile.SetPrefInt("dashTutorialSeen", 1);
            ProfileStorage.StorePlayerProfile(GameObject.FindWithTag("Player"), GameManager.Instance);
        }
    }
}
