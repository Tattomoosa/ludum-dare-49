using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float startFallDelay = 1.0f;
    public float loseDragSpeed = 4.0f;
    public float returnDelay = 10.0f;
    
    
    private Rigidbody _rigidbody;
    private float _initialDrag;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _initialDrag = _rigidbody.drag;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        Reset();
    }

    private void Update()
    {
        if (!_rigidbody.useGravity)
            return;
        if (_rigidbody.drag > 0)
            _rigidbody.drag -= loseDragSpeed * Time.deltaTime;
        if (_rigidbody.drag < 0)
            _rigidbody.drag = 0;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
            StartCoroutine(WaitThenStartToFall());
    }

    private IEnumerator WaitThenStartToFall()
    {
        yield return new WaitForSeconds(startFallDelay);
        StartToFall();
        StartCoroutine(WaitThenReset());
    }
    
    private IEnumerator WaitThenReset()
    {
        yield return new WaitForSeconds(returnDelay);
        Reset();
    }

    private void Reset()
    {
        _rigidbody.drag = _initialDrag;
        transform.position = _originalPosition;
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        transform.rotation = _originalRotation;
    }

    private void StartToFall()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
    }
}
