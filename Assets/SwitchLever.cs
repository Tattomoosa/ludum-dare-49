using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLever : MonoBehaviour
{
    public Vector3 endRotationEuler;
    public void DoSwitchLever()
    {
        StartCoroutine(SwitchLeverCoroutine());
    }

    private IEnumerator SwitchLeverCoroutine()
    {
        var t = transform;
        var startRotation = t.rotation;
        var endRotation = Quaternion.Euler(endRotationEuler);

        var progress = 0.0f;
        
        // Debug.Log($"SWITCHING from {startRotationX} to {startRotationX * 2}");
        while (progress < 1.0f)
        {
            t.rotation = Quaternion.Lerp(startRotation, endRotation, progress);
            // t.Rotate(Vector3.right, 50.0f * Time.deltaTime);
            progress += 1.0f * Time.deltaTime;
            yield return null;
        }
    }
    
    
}
