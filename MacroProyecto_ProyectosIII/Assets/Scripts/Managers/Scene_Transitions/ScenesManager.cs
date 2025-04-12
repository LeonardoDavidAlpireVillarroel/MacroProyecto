using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    public Slider progressBar;
    public GameObject transitionsContainer;

    private SceneTransition[] transitions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (transitionsContainer != null)
        {
            transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
        }
    }

    public void LoadScene(string sceneName, string transitionName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        SceneTransition transition = null;

        if (transitions != null && transitions.Length > 0)
        {
            transition = transitions.FirstOrDefault(t => t.name == transitionName);
        }

        if (transition == null)
        {
            yield break;
        }

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        yield return transition.AnimateTransitionIn();

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
        }

        do
        {
            if (progressBar != null)
                progressBar.value = scene.progress;

            yield return null;
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;

        if (progressBar != null)
            progressBar.gameObject.SetActive(false);

        yield return transition.AnimateTransitionOut();
    }
}
