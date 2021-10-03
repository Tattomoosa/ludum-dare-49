using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignLooker : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    public LayerMask signLayer;
    
    private void Update()
    {
        if (!Input.GetMouseButtonDown(1))
            return;
        
        var dir = transform.forward;
        var ray =
            new Ray(transform.position, dir);
        if (!Physics.Raycast(ray, out var hit, 10.0f, signLayer))
        {
            Debug.Log("Raycast hit nothing.");
            return;
        }
        else
        {
            var sign = hit.collider.attachedRigidbody.GetComponent<Sign>();
            if (!sign)
                return;
            dialogueUI.ShowDialogue(sign.Textboxes);
        }
    }
}
