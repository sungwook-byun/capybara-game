using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;

public class StripShaders : IPreprocessShaders
{
    public int callbackOrder => 0;

    public void OnProcessShader(UnityEngine.Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    {
        string name = shader.name;

        // 제거할 Shader 이름 목록
        if (name.Contains("Hidden/CoreSRP/CoreCopy") || name.Contains("Hidden/Universal/HDRDebugView"))
        {
            data.Clear(); // 빌드에서 완전히 제외
            UnityEngine.Debug.Log($"[Shader Strip] Removed shader: {name}");
        }
    }
}
