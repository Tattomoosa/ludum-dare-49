using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTime
{
    public static bool IsPaused { get; private set; } = false;

    public static void Pause()
    {
        IsPaused = true;
    }
    public static void Unpause()
    {
        IsPaused = false;
    }
    
    public static float DeltaTime => IsPaused ? 0 : Time.deltaTime;
}
