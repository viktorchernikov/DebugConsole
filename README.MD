# Viktor Chernikov's Debug Console
Somewhat stupid debug console implementation

## Initialization
```csharp
private void Init()
{
    int count = DebugConsole.LoadAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    DebugConsole.OnLog += OnLog;
    DebugConsole.Log($"Loaded {count} console commands.");
    DebugConsole.LogUnity = true;
}
private void OnLog(LogMessage message)
{
    // do something with it
}
```

## Creating commands
```csharp
using System;
using UnityEngine;
using VCUE;

public class SampleCommands
{
    [ConsoleCommand(Name: "teleport", Description = "A command that allows you to teleport")]
    public static void OnTeleportCmd(Vector3 worldPosition)
    {
        GameObject go = GameObject.Find("Player");
        if (go != null)
        {
            go.transform.position = worldPosition;
            DebugConsole.Log(message: "Teleported", source: "Console", severity: LogSeverity.Info);
        }
    }

    [ConsoleCommand(Name: "time")]
    public static void OnTimeCmd()
    {
        Debug.Log(DateTime.Now);
    }
}
```