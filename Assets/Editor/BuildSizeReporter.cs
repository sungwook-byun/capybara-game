using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class BuildIncludedSizeReporter : EditorWindow
{
    [MenuItem("Tools/Build Included Size Report")]
    public static void ShowWindow()
    {
        GetWindow<BuildIncludedSizeReporter>("Build Included Size Report");
    }

    private Vector2 scroll;
    private List<(string, long)> assetList = new List<(string, long)>();

    private void OnGUI()
    {
        GUILayout.Label("빌드 포함 에셋 용량 분석", EditorStyles.boldLabel);

        if (GUILayout.Button("분석 시작 (WebGL 빌드 설정 기준)"))
        {
            AnalyzeBuildAssets();
        }

        GUILayout.Space(10);

        if (assetList.Count > 0)
        {
            GUILayout.Label("빌드 포함 에셋 (큰 순)", EditorStyles.boldLabel);
            scroll = GUILayout.BeginScrollView(scroll);

            foreach (var asset in assetList)
            {
                GUILayout.Label($"{asset.Item1}  -  {FormatBytes(asset.Item2)}");
            }

            GUILayout.EndScrollView();
        }
    }

    private void AnalyzeBuildAssets()
    {
        assetList.Clear();

        // 현재 Build Settings에 포함된 씬 기준으로 빌드 에셋 목록 가져오기
        string[] scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        string[] includedAssets = AssetDatabase.GetDependencies(scenes, true);

        foreach (string path in includedAssets)
        {
            if (!path.StartsWith("Assets/")) continue; // 외부 제외
            string fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                long size = new FileInfo(fullPath).Length;
                assetList.Add((path, size));
            }
        }

        assetList.Sort((a, b) => b.Item2.CompareTo(a.Item2));
        Debug.Log("빌드 포함 에셋 용량 분석 완료");
    }

    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
