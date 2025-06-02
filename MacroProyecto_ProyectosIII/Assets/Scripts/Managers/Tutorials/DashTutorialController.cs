using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DashTutorialController : MonoBehaviour
{
    public CanvasGroup dashTutorialCanvasGroup;
    public Button closeButton;

    private bool dashTutorialShown = false;

    public LevelController levelController;

    void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseDashTutorial);
        }
    }

    void Start()
    {
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

        float delayBeforePause = 1f;
        yield return new WaitForSecondsRealtime(delayBeforePause);

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

        Time.timeScale = 1f;

        ProfileStorage.s_currentProfile.SetPrefInt("dashTutorialSeen", 1);
        ProfileStorage.StorePlayerProfile(GameObject.FindWithTag("Player"), GameManager.Instance);

        if (levelController != null)
        {
            levelController.StartIntroAfterDash();
            levelController?.ForzarInicioNivelDesdeDashTutorial();
        }
    }

    private void HideDashTutorialInstant()
    {
        if (dashTutorialCanvasGroup != null)
        {
            dashTutorialCanvasGroup.alpha = 0f;
            dashTutorialCanvasGroup.interactable = false;
            dashTutorialCanvasGroup.blocksRaycasts = false;
            dashTutorialShown = false;
        }
    }
}
