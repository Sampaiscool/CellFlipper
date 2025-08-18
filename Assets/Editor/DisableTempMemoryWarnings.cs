#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class DisableTempMemoryWarnings
{
    static DisableTempMemoryWarnings()
    {
        // Prevent Editor from logging harmless temp memory leaks
        Debug.unityLogger.logEnabled = true; // keep normal logs
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
    }
}
#endif
