using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace VCUE
{
    public static class DebugConsole
    {
        public static IReadOnlyCollection<string> Logs => (IReadOnlyCollection<string>)savedLogs;

        public static readonly Version Version = new Version(major: 1, minor: 1);

        public static event Action<LogMessage> OnLog;
        public static event Action OnClearLogs;
        public static event Action OnUpdate;
        public static event Action OnCommand;

        public static bool LogUnity
        {
            get => logUnity;
            set
            {
                if (logUnity != value) 
                {
                    logUnity = value;
                    switch (logUnity)
                    {
                        case true:
                            Application.logMessageReceived += OnUnityLog;
                            break;
                        case false:
                            Application.logMessageReceived -= OnUnityLog;
                            break;
                    }
                }
            }
        }

        public static void Log(string message) => Log(message, null);
        public static void Log(string message, string source) => Log(message, source, LogSeverity.Info);
        public static void Log(string message, string source, LogSeverity severity = LogSeverity.Info) => Log(message, source, DateTime.Now, severity);
        public static void Log(string message, string source, DateTime timestamp, LogSeverity severity = LogSeverity.Info)
        {
            LogMessage msg = new LogMessage(message, source, timestamp, severity);
            savedLogs.Add(msg);
            OnLog?.Invoke(msg);
            OnUpdate?.Invoke();
        }
        public static void ClearLogs()
        {
            savedLogs.Clear();
            OnClearLogs?.Invoke();
            OnUpdate?.Invoke();
        }
        public static bool InvokeCommand(string name, params object[] args)
        {
            if (cachedMethods.ContainsKey(name))
            {
                InvokeMethod(cachedMethods[name], args);
                OnCommand?.Invoke();
                OnUpdate?.Invoke();
                return true;
            }
            return false;
        }
        public static int LoadAssemblies(Assembly[] assemblies)
        {
            cachedMethods.Clear();
            int count = 0;
            foreach (Assembly asm in assemblies)
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (!type.IsClass)
                    {
                        continue;
                    }
                    foreach (var method in type.GetMethods())
                    {
                        ConsoleCommand atb = method.GetCustomAttribute<ConsoleCommand>(false);
                        if (atb != null)
                        {
                            if (cachedMethods.ContainsKey(atb.Name))
                            {
                                throw new InvalidCommandException($"Encountered trouble while loading console commands from assemblies - can't declare multiple commands with the same name! See {method.DeclaringType} and {cachedMethods[atb.Name].DeclaringType} classes");
                            }
                            if (!method.IsStatic)
                            {
                                throw new InvalidCommandException($"Encountered trouble while loading console command - every console command should be static! See {method.DeclaringType}");
                            }
                            if (atb.Name == null)
                            {
                                throw new InvalidCommandException($"Encountered trouble while loading console command - command name can't be empty! See {method.DeclaringType}");
                            }
                            count++;
                            cachedMethods.Add(atb.Name, method);
                        }
                    }
                }
            }
            if (count > 0)
            {
                OnUpdate?.Invoke();
            }
            return count;
        }


        private static void InvokeMethod(MethodInfo method, params object[] parameters) => method.Invoke(null, parameters);
        private static void OnUnityLog(string condition, string stackTrace, LogType type)
        {
            LogSeverity severity = LogSeverity.Info;
            switch (type)
            {
                case LogType.Log:
                    severity = LogSeverity.Debug;
                    break;
                case LogType.Exception:
                    severity = LogSeverity.Error;
                    break;
                case LogType.Warning:
                    severity = LogSeverity.Warning;
                    break;
                case LogType.Assert:
                    severity = LogSeverity.Warning;
                    break;
            }
            Log(condition, "Unity", severity);
        }


        private static bool logUnity = false;

        private static readonly Dictionary<string, MethodInfo> cachedMethods = new Dictionary<string, MethodInfo>();
        private static readonly List<LogMessage> savedLogs = new List<LogMessage>();
    }
}