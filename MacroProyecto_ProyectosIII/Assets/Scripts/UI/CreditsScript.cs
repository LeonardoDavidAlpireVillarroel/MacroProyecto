using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> panels;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float displayDuration = 2.0f;

    private void Start()
    {
        foreach (var panel in panels)
        {
            panel.alpha = 0f;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            CanvasGroup currentPanel = panels[i];

            yield return StartCoroutine(FadeCanvasGroup(currentPanel, 0f, 1f));

            if (i < panels.Count - 1)
            {
                yield return new WaitForSeconds(displayDuration);

                yield return StartCoroutine(FadeCanvasGroup(currentPanel, 1f, 0f));
            }
            else
            {
                currentPanel.interactable = true;
                currentPanel.blocksRaycasts = true;
            }
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end)
    {
        float elapsed = 0f;

        canvasGroup.interactable = end > 0f;
        canvasGroup.blocksRaycasts = end > 0f;

        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = end;
    }

    public void PlaySFX(string soundName)
    {
        SoundManager.Instance?.PlaySound2D(soundName);
    }

    public void GoClaroExitGame()
    {
        Time.timeScale = 1;
        if (ScenesManager.Instance != null)
            ScenesManager.Instance.LoadScene("ClaroPacifico", "CrossFade");

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic("ClaroPacifico");
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1;
        if (ScenesManager.Instance != null)
            ScenesManager.Instance.LoadScene("MainMenu", "CrossFade");

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
