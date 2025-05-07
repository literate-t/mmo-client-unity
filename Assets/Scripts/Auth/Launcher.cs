using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public static string AuthToken { get; private set; }

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
        Dictionary<string, string> query = uri.Query
            .TrimStart('?')
            .Split('&')
            .Select(param => param.Split('='))
            .ToDictionary(item => item[0], item => Uri.UnescapeDataString(item[1]));

        string result;
        if (!query.TryGetValue("token", out result))
        {
            Debug.LogError("Launcer: Parsing token failed");
            ForceQuit();
            return;
        }

        AuthToken = result;

        DontDestroyOnLoad(this);
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
