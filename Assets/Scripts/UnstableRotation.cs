using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class UnstableRotation : MonoBehaviour
{
    public bool debugDraw = false;
    public float minNewRotationTargetTime = 0.1f;
    public float maxNewRotationTargetTime = 0.8f;
    public float rotationRadius = 0.1f;
    public float lookSpeed = 0.1f;

    private Vector3 _rotationTarget = Vector3.forward;
    private Transform _parent;

    private void Start()
    {
        _parent = transform.parent;
        StartCoroutine(UpdateRotationTarget());
    }

    private void Update()
    {
        var targetRotation = Quaternion.LookRotation(_rotationTarget, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * GameTime.DeltaTime);
        if (debugDraw)
            DebugDraw();
    }

    private void DebugDraw()
    {
        Debug.Log(_rotationTarget);
        Debug.DrawLine(
            transform.position + _rotationTarget + Vector3.up,
            transform.position + _rotationTarget + Vector3.down
        );
        Debug.DrawLine(
            transform.position + _rotationTarget + Vector3.left,
            transform.position + _rotationTarget + Vector3.right
        );
    }

    private IEnumerator UpdateRotationTarget()
    {
        while (true)
        {
            _rotationTarget = _parent.forward; // Vector3.forward;
            _rotationTarget.y += Random.Range(0.0f, rotationRadius);
            _rotationTarget.x += Random.Range(0.0f, rotationRadius);
            yield return new WaitForSeconds(Random.Range(minNewRotationTargetTime, maxNewRotationTargetTime));
        }
    }
}
