using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    /// <summary>
    /// Debug 모드에서만 출력을 하기 위한 Editor 함수
    /// </summary>
    public static class DebugEditor
    {
        // reference
        // https://github.com/prasetion/DisableLoggingReleaseBuild
        // https://nanalistudios.tistory.com/8

        public const string LOGGER_SYMBOL = "UNITY_EDITOR";

        [System.Diagnostics.Conditional(LOGGER_SYMBOL)]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        [System.Diagnostics.Conditional(LOGGER_SYMBOL)]
        public static void Log(object message, Object context)
        {
            Debug.Log(message, context);
        }

        [System.Diagnostics.Conditional(LOGGER_SYMBOL)]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        [System.Diagnostics.Conditional(LOGGER_SYMBOL)]
        public static void LogError(object message, Object context)
        {
            Debug.LogError(message, context);
        }
    }
}
