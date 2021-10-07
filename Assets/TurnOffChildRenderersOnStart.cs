using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffChildRenderersOnStart : MonoBehaviour
{
    void Start()
    {
        foreach (var childRenderer in GetComponentsInChildren<Renderer>())
            childRenderer.enabled = false;
    }
}
