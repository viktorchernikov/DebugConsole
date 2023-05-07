# Viktor Chernikov's Debug Console
Somewhat stupid debug console implementation

## Installation
Debug console can be installed with the Unity Package Manager. Please follow the instructions in the manual about [Installing a package from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html).

Use the following URL in the package manager to add a transport via git URL. Change `dev` to any of the stable versions in the repository's branches to choose which version would you like to add:<br>
https://github.com/viktorchernikov/DebugConsole.git#dev

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