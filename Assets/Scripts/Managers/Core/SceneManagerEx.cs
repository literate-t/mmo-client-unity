using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene
    {
        get
        {
            return GameObject.FindObjectOfType<BaseScene>();
        }
    }

    public void LoadScene(Define.Scene sceneType)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(sceneType));
    }

    string GetSceneName(Define.Scene sceneType)
    {
        return Enum.GetName(typeof(Define.Scene), sceneType);
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
