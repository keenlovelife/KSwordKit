using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSwordKit
{
    /// <summary>
    /// KSwordKit 内部 debug类
    /// <para>函数多是Debug类的函数</para>
    /// </summary>
    public class KitDebug
    {
        public static bool logEnabled { get; set; }

        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            if (!logEnabled) return;
            bool depthTest = true;
            DrawLine(start, end, color, duration, depthTest);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            if (!logEnabled) return;
            bool depthTest = true;
            float duration = 0f;
            DrawLine(start, end, color, duration, depthTest);
        }

        public static void DrawLine(Vector3 start, Vector3 end)
        {
            if (!logEnabled) return;
            bool depthTest = true;
            float duration = 0f;
            Color white = Color.white;
            DrawLine(start, end, white, duration, depthTest);
        }

        public static void DrawLine(Vector3 start, Vector3 end, [UnityEngine.Internal.DefaultValue("Color.white")] Color color, [UnityEngine.Internal.DefaultValue("0.0f")] float duration, [UnityEngine.Internal.DefaultValue("true")] bool depthTest)
        {
            if (!logEnabled) return;
            Debug.DrawLine(start, end, color, duration, depthTest);
        }

        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
        {
            if (!logEnabled) return;
            bool depthTest = true;
            DrawRay(start, dir, color, duration, depthTest);
        }

        public static void DrawRay(Vector3 start, Vector3 dir, Color color)
        {
            if (!logEnabled) return;
            bool depthTest = true;
            float duration = 0f;
            DrawRay(start, dir, color, duration, depthTest);
        }

        public static void DrawRay(Vector3 start, Vector3 dir)
        {
            if (!logEnabled) return;
            bool depthTest = true;
            float duration = 0f;
            Color white = Color.white;
            DrawRay(start, dir, white, duration, depthTest);
        }

        public static void DrawRay(Vector3 start, Vector3 dir, [UnityEngine.Internal.DefaultValue("Color.white")] Color color, [UnityEngine.Internal.DefaultValue("0.0f")] float duration, [UnityEngine.Internal.DefaultValue("true")] bool depthTest)
        {
            if (!logEnabled) return;
            DrawLine(start, start + dir, color, duration, depthTest);
        }

        public static void Break()
        {
            if (!logEnabled) return;

            Debug.Break();
        }

        public static void DebugBreak()
        {
            if (!logEnabled) return;

            Debug.DebugBreak();
        }

        public static void Log(object message)
        {
            if (!logEnabled) return;
            Debug.unityLogger.Log(LogType.Log, message);
        }

        public static void Log(object message, Object context)
        {
            if (!logEnabled) return;

            Debug.unityLogger.Log(LogType.Log, message, context);
        }

        public static void LogFormat(string format, params object[] args)
        {
            if (!logEnabled) return;

            Debug.unityLogger.LogFormat(LogType.Log, format, args);
        }

        public static void LogFormat(Object context, string format, params object[] args)
        {
            if (!logEnabled) return;

            Debug.unityLogger.LogFormat(LogType.Log, context, format, args);
        }

        public static void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args)
        {
            if (!logEnabled) return;

            Debug.LogFormat(logType, logOptions, context, format, args);
        }

        public static void LogError(object message)
        {
            if (!logEnabled) return;

            Debug.unityLogger.Log(LogType.Error, message);
        }

        public static void LogError(object message, Object context)
        {
            if (!logEnabled) return;

            Debug.unityLogger.Log(LogType.Error, message, context);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            if (!logEnabled) return;

            Debug.unityLogger.LogFormat(LogType.Error, format, args);
        }

        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
            if (!logEnabled) return;

            Debug.unityLogger.LogFormat(LogType.Error, context, format, args);
        }

        public static void ClearDeveloperConsole()
        {
            if (!logEnabled) return;

            Debug.ClearDeveloperConsole();
        }

        public static void LogException(System.Exception exception)
        {
            if (!logEnabled) return;

            Debug.unityLogger.LogException(exception, null);
        }

        public static void LogException(System.Exception exception, Object context)
        {
            if (!logEnabled) return;

            Debug.unityLogger.LogException(exception, context);
        }

        public static void LogWarning(object message)
        {
            if (!logEnabled) return;

            Debug.unityLogger.Log(LogType.Warning, message);
        }

        public static void LogWarning(object message, Object context)
        {
            if (!logEnabled) return;

            Debug.unityLogger.Log(LogType.Warning, message, context);
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            if (!logEnabled) return;

            Debug.unityLogger.LogFormat(LogType.Warning, format, args);
        }

        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
            if (!logEnabled) return;

            Debug.unityLogger.LogFormat(LogType.Warning, context, format, args);
        }


        public static void Assert(bool condition)
        {
            if (!logEnabled) return;
            
            if (!condition)
            {
                Debug.unityLogger.Log(LogType.Assert, "Assertion failed");
            }
        }

        public static void Assert(bool condition, Object context)
        {
            if (!logEnabled) return;

            if (!condition)
            {
                Debug.unityLogger.Log(LogType.Assert, (object)"Assertion failed", context);
            }
        }

        public static void Assert(bool condition, object message)
        {
            if (!logEnabled) return;

            if (!condition)
            {
                Debug.unityLogger.Log(LogType.Assert, message);
            }
        }

        public static void Assert(bool condition, string message)
        {
            if (!logEnabled) return;

            if (!condition)
            {
                Debug.unityLogger.Log(LogType.Assert, message);
            }
        }

        public static void Assert(bool condition, object message, Object context)
        {
            if (!logEnabled) return;

            if (!condition)
            {
                Debug.unityLogger.Log(LogType.Assert, message, context);
            }
        }

        public static void Assert(bool condition, string message, Object context)
        {
            if (!logEnabled) return;

            if (!condition)
            {
                Debug.unityLogger.Log(LogType.Assert, (object)message, context);
            }
        }

        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            if (!logEnabled) return;

            if (!condition)
            {
                Debug.unityLogger.LogFormat(LogType.Assert, format, args);
            }
        }

        public static void AssertFormat(bool condition, Object context, string format, params object[] args)
        {
            if (!logEnabled) return;

            if (!condition)
            {
                Debug.unityLogger.LogFormat(LogType.Assert, context, format, args);
            }
        }

        public static void LogAssertion(object message)
        {
            if (!logEnabled) return;

            Debug.unityLogger.Log(LogType.Assert, message);
        }

        public static void LogAssertion(object message, Object context)
        {
            if (!logEnabled) return;

            Debug.unityLogger.Log(LogType.Assert, message, context);
        }

        public static void LogAssertionFormat(string format, params object[] args)
        {
            if (!logEnabled) return;

            Debug.unityLogger.LogFormat(LogType.Assert, format, args);
        }


        public static void LogAssertionFormat(Object context, string format, params object[] args)
        {
            if (!logEnabled) return;

            Debug.unityLogger.LogFormat(LogType.Assert, context, format, args);
        }

    }
}
