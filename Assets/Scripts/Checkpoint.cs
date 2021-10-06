using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private Light checkpointLight;
    public void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (!player)
            return;
        player.SetCheckpoint(this);
        checkpointLight.gameObject.SetActive(true);
    }
    
    

}
