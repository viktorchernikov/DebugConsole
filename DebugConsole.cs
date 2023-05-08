using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace VCUE
{
    public static class DebugConsole
    {
        public static IReadOnlyCollection<LogMessage> Logs => savedLogs;

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
        public static bool LogCommands = true;
        public static bool ShowTimestamp = false;
        public static bool ShowSource = false;
        public static StringComparison FindComparison = StringComparison.InvariantCultureIgnoreCase;

        public static void Log(string message) => Log(message, null);
        public static void Log(string message, string source) => Log(message, source, LogSeverity.Info);
        public static void Log(string message, LogSeverity severity) => Log(message, null, severity);
        public static void Log(string message, string source, LogSeverity severity = LogSeverity.Info) => Log(message, source, DateTime.Now, severity);
        public static void Log(string message, DateTime timestamp, LogSeverity severity = LogSeverity.Info) => Log(message, null, timestamp, severity);
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

        #region Command invoking
        public static bool InvokeCommand(string name, params object[] args)
        {
            if (HasCommand(name))
            {
                return InvokeCommand(cmds[name]);
            }
            return false;
        }
        public static bool InvokeCommand(ConsoleCommandInfo command, params object[] args)
        {
            if (LogCommands)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("> ");
                sb.Append(command.Name);
                foreach (object arg in args)
                {
                    sb.Append(" ");
                    sb.Append(arg);
                }
                Log(sb.ToString(), LogSeverity.Info);
            }
            try
            {
                command.MethodRef.Invoke(null, args);
                return true;
            }
            catch (Exception e)
            {
                Log(e.Message, "Console", LogSeverity.Error);
                return false;
            }
        }
        #endregion
        #region Command Retrieving
        public static bool HasCommand(string name)
        {
            return cmdNames.Contains(name);
        }
        public static bool IsValid(ConsoleCommandInfo command)
        {
            return cmds.ContainsValue(command);
        }
        public static ConsoleCommandInfo GetCommand(string name)
        {
            return cmds[name];
        }
        public static bool TryGetCommand(string name, out ConsoleCommandInfo result)
        {
            if (HasCommand(name))
            {
                result = GetCommand(name);
                return true;
            }
            result = null;
            return false;
        }
        public static ConsoleCommandInfo FindCommand(string name)
        {
            ConsoleCommandInfo command;
            if (!TryGetCommand(name, out command))
            {
                foreach(var cmdn in cmdNames)
                {
                    if (cmdn.Contains(name, FindComparison))
                    {
                        command = GetCommand(cmdn);
                    }
                }
            }
            return command;
        }
        public static bool TryFindCommand(string name, out ConsoleCommandInfo result)
        {
            result = FindCommand(name);
            return result != null;
        }
        public static IReadOnlyCollection<ConsoleCommandInfo> FindCommands(string name)
        {
            var list = new List<ConsoleCommandInfo>();
            foreach (var cn in cmdNames)
            {
                ConsoleCommandInfo c;
                if (TryFindCommand(name, out c))
                {
                    list.Add(c);
                }
            }
            return list;
        }
        #endregion

        public static int LoadAssemblies(Assembly[] assemblies)
        {
            cmds.Clear();
            cmdNames.Clear();
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
                            var name = atb.Name;

                            if (cmds.ContainsKey(atb.Name))
                            {
                                throw new InvalidCommandException($"Encountered trouble while loading console commands from assemblies - can't declare multiple commands with the same name! See {method.DeclaringType} and {cmds[name].MethodRef.DeclaringType} classes");
                            }
                            if (!method.IsStatic)
                            {
                                throw new InvalidCommandException($"Encountered trouble while loading console command - every console command should be static! See {method.DeclaringType}");
                            }
                            if (atb.Name == null)
                            {
                                throw new InvalidCommandException($"Encountered trouble while loading console command - command name can't be empty! See {method.DeclaringType}");
                            }

                            ConsoleCommandInfo cmd = new ConsoleCommandInfo(name, method);
                            cmds.Add(name, cmd);
                            cmdNames.Add(name);
                            count++;
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

        private static readonly Dictionary<string, ConsoleCommandInfo> cmds = new Dictionary<string, ConsoleCommandInfo>();
        private static readonly HashSet<string> cmdNames = new HashSet<string>();
        private static readonly List<LogMessage> savedLogs = new List<LogMessage>();
    }
}