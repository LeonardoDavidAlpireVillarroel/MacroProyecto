using System.Collections;

using UnityEngine;

public class InventoryFullMessage : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Para fade
    public Transform messageTransform; // Para animación de escala
    public float showDuration = 2f;
    public float fadeDuration = 0.3f;
    public float popScale = 1.2f; // Escala al aparecer

    private Coroutine currentRoutine;

    public void ShowMessage()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowAndAnimate());
    }

    private IEnumerator ShowAndAnimate()
    {
        // Asegura que está visible y en escala mínima
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(true);
        messageTransform.localScale = Vector3.zero;

        // Mostrar (Fade in y pop)
        yield return StartCoroutine(FadeCanvasGroup(0, 1, fadeDuration));
        yield return StartCoroutine(ScaleObject(Vector3.zero, Vector3.one * popScale, fadeDuration * 0.5f));
        yield return StartCoroutine(ScaleObject(Vector3.one * popScale, Vector3.one, fadeDuration * 0.2f));

        // Espera
        yield return new WaitForSeconds(showDuration);

        // Ocultar (pop inverso + fade out)
        yield return StartCoroutine(ScaleObject(Vector3.one, Vector3.zero, fadeDuration * 0.3f));
        yield return StartCoroutine(FadeCanvasGroup(1, 0, fadeDuration));
        canvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvasGroup(float start, float end, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    private IEnumerator ScaleObject(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            messageTransform.localScale = Vector3.Lerp(from, to, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        messageTransform.localScale = to;
    }
}
