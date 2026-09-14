using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour, IService
{
    public string currentSceneName = null;

    public Action<Scene, LoadSceneMode> onLoadAction;

    private GameObject _fadeCanvas;

    public void Init()
    {
        if (CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<ConfigMain>())!.Get<ConfigMain>().showFading)
            CreateFadeCanvas();

        currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += (scene, sceneMode) => onLoadAction?.Invoke(scene, sceneMode);
        StartCoroutine(Unfade(0.5f));
    }

    public void Load(string sceneName, float fadeSpeed = 0.5f) => StartCoroutine(LoadSceneCoroutine(sceneName, fadeSpeed));

    private IEnumerator LoadSceneCoroutine(string sceneName, float fadeSpeed)
    {
        yield return Fade(fadeSpeed);
        LoadScene(sceneName);
        yield return Unfade(fadeSpeed);
    }

    private void LoadScene(string sceneName)
    {
        if (currentSceneName == null) return;
        SceneManager.LoadScene(sceneName);
        currentSceneName = sceneName;
    }

    private void CreateFadeCanvas()
    {
        _fadeCanvas = new GameObject("Canvas - FadeCanvas");
        DontDestroyOnLoad(_fadeCanvas);
        _fadeCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        _fadeCanvas.GetComponent<Canvas>().sortingOrder = 1000;

        _fadeCanvas.AddComponent<GraphicRaycaster>();

        GameObject fadeImage = new GameObject("FadeImage");
        fadeImage.transform.parent = _fadeCanvas.transform;

        fadeImage.AddComponent<Image>().color = Color.black;

        fadeImage.GetComponent<RectTransform>().sizeDelta = new Vector2(100000, 100000);
    }

    private IEnumerator Fade(float duration)
    {
        if (_fadeCanvas == null)
        {
            yield return null;
            yield break;
        }

        _fadeCanvas.GetComponentInChildren<Image>().raycastTarget = true;
        yield return _fadeCanvas.transform.GetChild(0).GetComponent<Image>().DOFade(1, duration)
            .WaitForCompletion();
    }

    private IEnumerator Unfade(float duration)
    {
        if (_fadeCanvas == null)
        {
            yield return null;
            yield break;
        }

        yield return _fadeCanvas.transform.GetChild(0).GetComponent<Image>().DOFade(0, duration)
            .OnComplete(() => _fadeCanvas.GetComponentInChildren<Image>().raycastTarget = false)
            .WaitForCompletion();
    }
}
