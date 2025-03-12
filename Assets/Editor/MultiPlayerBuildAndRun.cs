using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class MultiPlayerBuildAndRun
{
    [MenuItem("Tools/Run Multiplay/1 Players")]
    static void PerformWin64Build1()
    {
        PerformWin64Build(1);
    }

    [MenuItem("Tools/Run Multiplay/2 Players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }

    [MenuItem("Tools/Run Multiplay/3 Players")]
    static void PerformWin64Build3()
    {
        PerformWin64Build(3);
    }

    [MenuItem("Tools/Run Multiplay/4 Players")]
    static void PerformWin64Build4()
    {
        PerformWin64Build(4);
    }

    static void PerformWin64Build(int playerCount)
    {
        DeleteExistingFolders();

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        for (int i = 0; i < playerCount; i++)
        {
            string buildPath = $@"Builds\Win64\{(i + 1).ToString()}_{GetProjectName()}/{(i + 1).ToString()}_{GetProjectName()}.exe";
            BuildReport report = BuildPipeline.BuildPlayer(GetScenePaths(), buildPath, BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
        }
    }

    private static void DeleteExistingFolders()
    {
        string folderPath = @"D:\SourceCode\GameSource\MMO_Basic\UnityClient\Builds\Win64";

        foreach (string dir in Directory.GetDirectories(folderPath))
        {
            Directory.Delete(dir, true);
        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}
