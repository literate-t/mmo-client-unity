using System;
using System.Linq;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public static string AuthToken { get; private set; }
    public static string Id { get; private set; }    

    private void Awake()
    {
        string[] agrs = Environment.GetCommandLineArgs();
        string uriArg = agrs.FirstOrDefault(str => str.StartsWith("my2drpg://"));

        if (string.IsNullOrEmpty(uriArg))
        {
            Debug.LogError("Launcher: No Uri");
            ForceQuit();
            return;
        }

        Uri uri = new(uriArg);

        var queries = uri.Query
            .TrimStart('?')
            .Split('&')
            .Select(param => param.Split('='));

        var dic_query = queries.ToDictionary(
            item => item[0],
            item => item[1], StringComparer.OrdinalIgnoreCase);

        string result;
        if (!dic_query.TryGetValue("token", out result))
        {
            Debug.LogError("Launcher: Parsing token failed");
            ForceQuit();
            return;
        }

        AuthToken = Uri.UnescapeDataString(result);

        if (!dic_query.TryGetValue("email", out result))
        {
            Debug.LogError("Launcher: Parsing email failed");
            ForceQuit();
            return;
        }

        Id = result.Substring(0, result.IndexOf('@'));

        DontDestroyOnLoad(gameObject);

        AuthVerifier authVerifier = new();
        StartCoroutine(authVerifier.Verify());
    }

    public static void ForceQuit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();        
        #endif
    }
}
