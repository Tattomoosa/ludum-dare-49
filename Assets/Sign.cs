using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField] private List<string> textboxes;
    
    public List<string> Textboxes => new List<string>(textboxes);
}
