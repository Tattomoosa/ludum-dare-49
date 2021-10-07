using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public float turnOnTime = 1.0f;
    [SerializeField]
    private Light checkpointLight;

    private bool _lightFadedIn = false;
    public void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (!player)
            return;
        player.SetCheckpoint(this);
        if (_lightFadedIn)
            return;
        StartCoroutine(FadeInLight());
    }

    private IEnumerator FadeInLight()
    {
        _lightFadedIn = true;
        var finalIntensity = checkpointLight.intensity;
        checkpointLight.gameObject.SetActive(true);
        var normalizedIntensity = 0.0f;
        while (normalizedIntensity <= 1.0f)
        {
            checkpointLight.intensity = Mathf.Lerp(0, finalIntensity, normalizedIntensity);
            normalizedIntensity += Time.deltaTime / turnOnTime;
            yield return null;
        }
    }
    

}
