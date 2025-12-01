using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance { get; private set; }
    public Animator animator;
    public Image fadeImage;
    private string sceneToLoad;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (fadeImage == null)
            {
                fadeImage = GetComponentInChildren<Image>();
            }

            if (animator == null)
            {
                Debug.LogError("SceneTransition: Animator n�o encontrado! Arraste o Canvas com Animator no Inspector.");
                return;
            }

            if (fadeImage == null)
            {
                Debug.LogError("SceneTransition: Image n�o encontrado! Arraste o FadeImage no Inspector.");
                return;
            }

            SetFadeAlpha(0f);

            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                canvas.enabled = true;
                canvas.gameObject.SetActive(true);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeToScene(string sceneName)
    {
        sceneToLoad = sceneName;
        Time.timeScale = 1f;
        Debug.Log($"Iniciando transi��o para: {sceneName}");

        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.enabled = true;
            canvas.gameObject.SetActive(true);
        }

        SetFadeAlpha(0f);

        animator.SetTrigger("FadeOut");
    }

    public void OnFadeOutComplete()
    {
        Debug.Log("FadeOut completado!");

        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Canvas c in allCanvases)
        {
            if (c.transform.IsChildOf(transform)) continue;

            c.enabled = false;
        }

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Carregando cena: {sceneToLoad}");

            SetFadeAlpha(1f);

            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.enabled = true;
            canvas.gameObject.SetActive(true);
        }

        SetFadeAlpha(1f);

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            SetFadeAlpha(1f);
            yield return null;
        }

        SetFadeAlpha(1f);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.15f);

        asyncLoad.allowSceneActivation = true;

        yield return null;
        SetFadeAlpha(1f);
    }

    public void TriggerFadeOutComplete()
    {
        OnFadeOutComplete();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Cena carregada: {scene.name}, iniciando FadeIn");
        StartCoroutine(DelayedFadeIn());
    }

    IEnumerator DelayedFadeIn()
    {
        yield return null;
        Debug.Log("Executando FadeIn");
        animator.SetTrigger("FadeIn");
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}