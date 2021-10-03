using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignLooker : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    public LayerMask signLayer;
    [SerializeField] private AudioSource _audioSource;
    
    private void Update()
    {
        if (!Input.GetMouseButtonDown(1) || dialogueUI.IsOpen)
            return;
        
        var dir = transform.forward;
        var ray =
            new Ray(transform.position, dir);
        if (!Physics.Raycast(ray, out var hit, 10.0f, signLayer))
        {
            Debug.Log("Raycast hit nothing.");
            return;
        }
        var sign = hit.collider.attachedRigidbody.GetComponent<Sign>();
        if (!sign)
            return;
        _audioSource.Play();
        dialogueUI.ShowDialogue(sign.Textboxes);
    }
}
