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
    // Обработка для itch.io
    string url = Application.absoluteURL;
    
    // Проверяем, запущена ли игра на itch.io
    if (url.Contains("itch.io") || url.Contains("itch.zone"))
    {
        // Перенаправляем на страницу игры на itch.io
        // Находим корневой URL (без параметров)
        Uri uri = new Uri(url);
        string baseUrl = $"{uri.Scheme}://{uri.Host}";
        
        // Если это поддомен itch.io
        if (uri.Host.Contains("itch.io") || uri.Host.Contains("itch.zone"))
        {
            // Нужно перезагрузить именно родительскую страницу
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
}
