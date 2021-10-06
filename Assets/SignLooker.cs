using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignLooker : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    public LayerMask signLayer;
    [SerializeField] private AudioSource audioSource;

    private bool _shouldLookForSign = false;
    
    private void Update()
    {
        if (dialogueUI.IsOpen || !GetInput())
            return;
        _shouldLookForSign = true;
        
    }

    private void FixedUpdate()
    {
        if (!_shouldLookForSign)
            return;

        _shouldLookForSign = false;
        var dir = transform.forward;
        var ray = new Ray(transform.position, dir);
        if (!Physics.Raycast(ray, out var hit, 50.0f, signLayer))
            return;
        
        var sign = hit.collider.attachedRigidbody.GetComponent<Sign>();
        if (!sign)
            return;
        
        audioSource.Play();
        dialogueUI.ShowDialogue(sign.Textboxes);
    }

    private bool GetInput()
    {
        return Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.R);
    }
}
