//https://github.com/itchio/butler.git

using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using TMPro.EditorUtilities;

public class UnityBuilder : MonoBehaviour
{
    //Paths to builds
    private static string projectName => Application.productName;
    private static string buildsRootPath => Path.Combine(Directory.GetCurrentDirectory(), "Builds");
    private static string buildWebPath => Path.Combine(buildsRootPath, "WebBuild");
    private static string buildWebZipPath => Path.Combine(buildsRootPath, projectName + "_WebGL.zip");
    
    //For Butler strings
    private static string itchUsername => "someshboy"; // Username on itch.io
    private static string itchGameName => "inkdarkunk"; // URL-slug of game
    private static string itchChannel => "web-build"; //channel to delivery
    
    [MenuItem("Build/3. GetCommand")]
    public static void GetCommand()
    {
        Debug.Log($"push \"{buildWebZipPath}\" {itchUsername}/{itchGameName}:{itchChannel}");
    }

    [MenuItem("Build/0. Build AND Push")]
    public static void BuildAndPush()
    {
        TMP_EditorCoroutine.StartCoroutine(BuildWebGLAndZipCoroutine(true));
    }

    [MenuItem("Build/1. WebGL Build and Zip")]
    public static void BuildWebGLAndZip()
    {
        TMP_EditorCoroutine.StartCoroutine(BuildWebGLAndZipCoroutine());
    }

    private static IEnumerator BuildWebGLAndZipCoroutine(bool isPush = false)
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Ожидание завершения компиляции
        while (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            yield return null;
        }
        
        if (!Directory.Exists(buildsRootPath))
        {
            Directory.CreateDirectory(buildsRootPath);
        }
        
        if (Directory.Exists(buildWebPath))
        {
            Directory.Delete(buildWebPath, true);
            Debug.Log("Удалена предыдущая папка сборки: " + buildWebPath);
        }
        
        // Даем время файловой системе обработать удаление
        yield return new WaitForSeconds(0.1f);
        
        if (File.Exists(buildWebZipPath))
        {
            File.Delete(buildWebZipPath);
            Debug.Log("Удален предыдущий ZIP-архив: " + buildWebZipPath);
        }
            
        // Особые конфигурации билда
        SetWebGLBuildSettings();
        
        // Сборка WebGL
        Debug.Log("Начинаем сборку WebGL...");
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetScenePaths(),
            locationPathName = buildWebPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };
        
        BuildPipeline.BuildPlayer(buildOptions);
        Debug.Log("Сборка WebGL завершена: " + buildWebPath);
        
        // Архивирование
        Debug.Log("Создаем ZIP-архив...");
        ZipFolder(buildWebPath, buildWebZipPath);
        Debug.Log("ZIP-архив создан: " + buildWebZipPath);

        if(isPush)
            PushToItch();
    }
    
    private static void SetWebGLBuildSettings()
    {
        try
        {
            // Явно устанавливаем нужные настройки
            //PlayerSettings.WebGL.template = "PROJECT:MyCustomTemplate"; // Замените на ваш шаблон
            // PlayerSettings.SplashScreen.show = false;
            Debug.Log("Настройки WebGL применены");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Не удалось применить некоторые настройки: {e.Message}");
        }
    }

    [MenuItem("Build/2. Push Zip on Itch.io")]
    public static void PushToItch()
    {
        try
        {
            if (!File.Exists(buildWebZipPath))
            {
                Debug.LogError($"ZIP файл не найден: {buildWebZipPath}");
                return;
            }
            
            // Формируем команду butler
            string butlerCommand = $"push \"{buildWebZipPath}\" {itchUsername}/{itchGameName}:{itchChannel}";
            
            Debug.Log($"Запускаем butler: {butlerCommand}");
            
            // Запускаем butler
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "butler",
                Arguments = butlerCommand,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            
            using (Process process = new Process())
            {
                process.StartInfo = processInfo;
                process.OutputDataReceived += (sender, e) => 
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.Log($"Butler: {e.Data}");
                };
                process.ErrorDataReceived += (sender, e) => 
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.LogError($"Butler error: {e.Data}");
                };
                
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                
                if (process.ExitCode == 0)
                    Debug.Log("Успешно загружено на itch.io!");
                else
                    Debug.LogError($"Ошибка загрузки. Код: {process.ExitCode}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при загрузке на itch.io: {e.Message}");
        }
    }

    private static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
            scenes[i] = EditorBuildSettings.scenes[i].path;

        return scenes;
    }
    
    private static void ZipFolder(string folderPath, string zipPath)
    {
        try
        {
            // Используем System.IO.Compression для создания ZIP
            if (File.Exists(zipPath))
                File.Delete(zipPath);
            
            System.IO.Compression.ZipFile.CreateFromDirectory(folderPath, zipPath);
            Debug.Log("ZIP создан успешно: " + zipPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Ошибка при создании ZIP: " + e.Message);
            
            // Альтернативный способ через 7-Zip (если установлен)
            TryZipWith7Zip(folderPath, zipPath);
        }
    }
    
    private static void TryZipWith7Zip(string folderPath, string zipPath)
    {
        try
        {
            string sevenZipPath = @"C:\Program Files\7-Zip\7z.exe";
            
            if (!File.Exists(sevenZipPath))
            {
                Debug.LogError("7-Zip не найден. Установите 7-Zip или проверьте путь.");
                return;
            }
            
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = sevenZipPath,
                Arguments = $"a \"{zipPath}\" \"{folderPath}\" -r",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            
            using (Process process = Process.Start(processInfo))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                    Debug.Log("ZIP создан через 7-Zip: " + zipPath);
                else
                    Debug.LogError("7-Zip вернул ошибку: " + process.StandardOutput.ReadToEnd());
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Ошибка при использовании 7-Zip: " + e.Message);
        }
    }
}
