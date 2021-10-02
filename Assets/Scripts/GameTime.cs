using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTime
{
    private static bool _isPaused = false;

    public static void Pause()
    {
        _isPaused = true;
    }
    public static void Unpause()
    {
        _isPaused = false;
    }
    
    public static float DeltaTime => _isPaused ? 0 : Time.deltaTime;
}
