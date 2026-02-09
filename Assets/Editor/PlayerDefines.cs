#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class DevDefines
{
    const string DEFINE = "SIMULATE_FIRST_RUN";

    [MenuItem("Dev/Simulate First Run/Toggle")]
    static void Toggle()
    {
        var group = EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

        if (defines.Contains(DEFINE))
        {
            defines = RemoveDefine(defines, DEFINE);
            Debug.Log("SIMULATE_FIRST_RUN OFF");
        }
        else
        {
            defines = AddDefine(defines, DEFINE);
            Debug.Log("SIMULATE_FIRST_RUN ON");
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
    }

    static string AddDefine(string defines, string define)
    {
        if (string.IsNullOrEmpty(defines)) return define;
        return defines.Contains(define) ? defines : defines + ";" + define;
    }

    static string RemoveDefine(string defines, string define)
    {
        return defines
            .Replace(";" + define, "")
            .Replace(define + ";", "")
            .Replace(define, "");
    }
}
#endif
