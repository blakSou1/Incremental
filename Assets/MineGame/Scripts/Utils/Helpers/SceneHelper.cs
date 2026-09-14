using UnityEngine;

[CreateAssetMenu(fileName = "SceneHelper", menuName = "Helpers/Scene")]
public class SceneHelper : ScriptableObject
{
    public void LoadScene(string sceneName)
    {
        G.SceneLoader.Load(sceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_WEBGL
        string url = Application.absoluteURL;

        if (url.Contains("itch.io") || url.Contains("itch.zone"))
        {
            string baseUrl = ExtractBaseUrl(url);

            if (baseUrl.Contains("itch.io") || baseUrl.Contains("itch.zone"))
            {
                ReloadItchPage();
            }
            else
            {
                Application.OpenURL(url);
            }
        }
        else
        {
            Application.OpenURL(url);
        }

#else
        Application.Quit();
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    /// <summary>
    /// Перезагружает страницу/iframe через встроенный JS.
    /// Работает без .jslib, используя Application.ExternalEval.
    /// </summary>
    private static void ReloadItchPage()
    {
        // Пытаемся перезагрузить родительскую страницу (top-level).
        // Если это запрещено cross-origin — перезагружаем текущий iframe.
        string js = @"
            try {
                if (window.top && window.top !== window.self) {
                    window.top.location.reload();
                } else {
                    window.location.reload();
                }
            } catch (e) {
                // Cross-origin — перезагружаем только свой iframe
                window.location.reload();
            }
        ";
        Application.ExternalEval(js);
    }
#endif

    /// <summary>
    /// Возвращает "{scheme}://{host}" без System.Uri (недоступен в части конфигураций WebGL).
    /// </summary>
    private static string ExtractBaseUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        int schemeEnd = url.IndexOf("://", System.StringComparison.Ordinal);
        if (schemeEnd < 0)
            return url;

        string scheme = url.Substring(0, schemeEnd);
        int hostStart = schemeEnd + 3;

        int hostEnd = url.IndexOf('/', hostStart);
        if (hostEnd < 0)
            hostEnd = url.Length;

        string host = url.Substring(hostStart, hostEnd - hostStart);
        return $"{scheme}://{host}";
    }
}