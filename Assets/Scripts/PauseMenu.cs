using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool IsPaused => gameObject.activeSelf;
    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(true);
        GameTime.Pause();
    }
    public void Unpause()
    {
        Cursor.lockState = CursorLockMode.None;
        GameTime.Unpause();
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
