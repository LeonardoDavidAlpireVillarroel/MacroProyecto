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

    private System.Action onCloseCallback;

    void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => CloseDashTutorial());
        }
    }

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => CloseDashTutorial());
        }
    }

    public void CheckAndShowDashTutorial()
    {
        if (dashTutorialShown) return;

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

    public void CloseDashTutorial(System.Action callback = null)
    {
        if (dashTutorialCanvasGroup != null && dashTutorialShown)
        {
            onCloseCallback = callback;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        dashTutorialShown = true;

        float initialDelay = 0.5f;
        yield return new WaitForSecondsRealtime(initialDelay);

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

        Time.timeScale = 0f;
    }

    private IEnumerator FadeOut()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        dashTutorialCanvasGroup.interactable = false;
        dashTutorialCanvasGroup.blocksRaycasts = false;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            dashTutorialCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        dashTutorialCanvasGroup.alpha = 0f;
        dashTutorialShown = false;

        if (onCloseCallback != null)
        {
            onCloseCallback.Invoke();
            onCloseCallback = null;
        }
        else
        {
            Time.timeScale = 1f;
            LevelController.Instance?.StartIntroAfterDash();
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
