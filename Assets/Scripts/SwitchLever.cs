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
        var startRotation = t.localRotation;
        var endRotation = Quaternion.Euler(endRotationEuler);

        var progress = 0.0f;
        
        while (progress < 1.0f)
        {
            t.localRotation = Quaternion.Lerp(startRotation, endRotation, progress);
            progress += 1.0f * Time.deltaTime;
            yield return null;
        }
    }
    
    
}
