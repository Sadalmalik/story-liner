// ReSharper disable UnusedMember.Global

// #define LOGGER_DISABLE
// #define LOG_LEVEL_ERROR
// #define LOG_LEVEL_WARNING
// #define LOG_LEVEL_INFO
// #define LOG_LEVEL_ALL

#if LOG_LEVEL_ALL || LOG_LEVEL_INFO || LOG_LEVEL_WARNING || LOG_LEVEL_ERROR
#define LOG_LEVEL_DEFINED
#endif

#if !LOG_LEVEL_DEFINED
#define LOG_LEVEL_ALL
#endif

#if LOGGER_DISABLE
#undef LOG_LEVEL_ERROR
#undef LOG_LEVEL_WARNING
#undef LOG_LEVEL_INFO
#undef LOG_LEVEL_ALL
#else
#if LOG_LEVEL_ERROR
#elif LOG_LEVEL_WARNING
#define LOG_LEVEL_ERROR
#elif LOG_LEVEL_INFO
#define LOG_LEVEL_ERROR
#define LOG_LEVEL_WARNING
#elif LOG_LEVEL_ALL
#define LOG_LEVEL_ERROR
#define LOG_LEVEL_WARNING
#define LOG_LEVEL_INFO
#endif
#endif

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Self.Architecture.Logger
{
    public static class Log
    {
        private static bool emptyTargets_ = true;
        private static LogTarget _defaultUnityTarget;
        private static LogTarget _defaultFileTarget;
        private static List<LogTarget> targets_;

        public static List<LogTarget> Targets
        {
            get
            {
                if (targets_ == null)
                    targets_ = new List<LogTarget>();
                return targets_;
            }
        }

        public static bool logTime = true;
        public static string timeFormat = "yyyy.MM.dd HH:mm:ss";
        public static string timeFileFormat = "yyyyMMddHHmmss";

        public static string GetDateTime()
        {
            return DateTime.Now.ToString(timeFormat);
        }

        public static string GetFileTime()
        {
            return DateTime.Now.ToString(timeFileFormat);
        }

        public static void AddUnityTarget()
        {
            emptyTargets_ = false;
            if (_defaultUnityTarget == null)
            {
                Targets.Add(_defaultUnityTarget = new LogTargetUnity());
            }
        }

        public static void AddDefaultFileTarget()
        {
            emptyTargets_ = false;
            if (_defaultFileTarget == null)
            {
                Targets.Add(_defaultFileTarget = new LogTargetFile($"logs.{GetFileTime()}.txt"));
            }
        }

        public static void AddTarget<T>() where T : LogTarget, new()
        {
            emptyTargets_ = false;
            Targets.Add(new T());
        }

        public static void AddTarget<T>(T target) where T : LogTarget
        {
            emptyTargets_ = false;
            Targets.Add(target);
        }

#if !LOG_LEVEL_ALL

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static void Temp(string message, params object[] args)
        {
#if !LOGGER_DISABLE && LOG_LEVEL_ALL
            if (emptyTargets_) return;

            string _final = args.Length > 0 ? string.Format(message, args) : message;
            string _message = $"{(logTime ? GetDateTime() + " " : "")}[TEMP] {_final}";
            foreach (var target in Targets)
                target.LogTemp(_message);
#endif
        }

#if !LOG_LEVEL_INFO

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static void Info(string message, params object[] args)
        {
#if !LOGGER_DISABLE && LOG_LEVEL_INFO
            if (emptyTargets_) return;

            string _final = args.Length > 0 ? string.Format(message, args) : message;
            string _message = $"{(logTime ? GetDateTime() + " " : "")}[INFO] {_final}";
            foreach (var target in Targets)
                target.LogInfo(_message);
#endif
        }

#if !LOG_LEVEL_WARNING

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static void Warning(string message, params object[] args)
        {
#if !LOGGER_DISABLE && LOG_LEVEL_WARNING
            if (emptyTargets_) return;

            string _final = args.Length > 0 ? string.Format(message, args) : message;
            string _message = $"{(logTime ? GetDateTime() + " " : "")}[WARNING] {_final}";
            foreach (var target in Targets)
                target.LogWarning(_message);
#endif
        }

#if !LOG_LEVEL_ERROR

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static void Error(string message, params object[] args)
        {
#if !LOGGER_DISABLE && LOG_LEVEL_ERROR
            if (emptyTargets_) return;

            string _final = args.Length > 0 ? string.Format(message, args) : message;
            string _message = $"{(logTime ? GetDateTime() + " " : "")}[ERROR] {_final}";
            foreach (var target in Targets)
                target.LogError(_message);
#endif
        }
    }
}