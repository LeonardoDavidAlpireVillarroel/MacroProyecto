using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DashTutorialController : MonoBehaviour
{
    public CanvasGroup dashTutorialCanvasGroup;
    public Button closeButton;

    private bool dashTutorialShown = false;

    void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseDashTutorial);
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Level2" || SceneManager.GetActiveScene().name == "Level3")
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

    public void CheckAndShowDashTutorial()
    {
        int dashTutorialSeen = ProfileStorage.s_currentProfile.GetPrefInt("dashTutorialSeen", 0);
        if (dashTutorialSeen == 0)
        {
            ShowDashTutorial();
        }
        else
        {
            HideDashTutorialInstant();
        }
    }

    public void ShowDashTutorial()
    {
        if (dashTutorialCanvasGroup != null && !dashTutorialShown)
        {
            gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    public void CloseDashTutorial()
    {
        if (dashTutorialCanvasGroup != null && dashTutorialShown)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        dashTutorialShown = true;
        Time.timeScale = 0f;

        dashTutorialCanvasGroup.alpha = 0f;
        dashTutorialCanvasGroup.interactable = false;
        dashTutorialCanvasGroup.blocksRaycasts = false;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            dashTutorialCanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        dashTutorialCanvasGroup.alpha = 1f;
        dashTutorialCanvasGroup.interactable = true;
        dashTutorialCanvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOut()
    {
        dashTutorialShown = false;
        dashTutorialCanvasGroup.interactable = false;
        dashTutorialCanvasGroup.blocksRaycasts = false;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            dashTutorialCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / duration));
            yield return null;
        }

        dashTutorialCanvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        Time.timeScale = 1f;

        ProfileStorage.s_currentProfile.SetPrefInt("dashTutorialSeen", 1);
        ProfileStorage.StorePlayerProfile(GameObject.FindWithTag("Player"), GameManager.Instance);
    }

    private void HideDashTutorialInstant()
    {
        if (dashTutorialCanvasGroup != null)
        {
            dashTutorialCanvasGroup.alpha = 0f;
            dashTutorialCanvasGroup.interactable = false;
            dashTutorialCanvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
            dashTutorialShown = false;
        }
    }
}
