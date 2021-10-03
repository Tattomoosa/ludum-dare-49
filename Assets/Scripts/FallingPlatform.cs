using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public AudioClip fallingSound;
    public float startFallDelay = 1.0f;
    public float loseDragSpeed = 4.0f;
    public float returnDelay = 10.0f;
    
    
    private Rigidbody _rigidbody;
    private float _initialDrag;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private AudioSource _audioSource;

    private bool _hasFallen = false;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _initialDrag = _rigidbody.drag;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _audioSource = GetComponent<AudioSource>();
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
        if (_hasFallen)
            return;
        if (other.gameObject.GetComponent<Player>())
            StartCoroutine(WaitThenStartToFall());
    }

    private IEnumerator WaitThenStartToFall()
    {
        _hasFallen = true;
        // play crack sound here? there's no delay now. crack might still be good
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
        var transform1 = transform;
        transform1.position = _originalPosition;
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        transform1.rotation = _originalRotation;
        _hasFallen = false;
    }

    private void StartToFall()
    {
        _audioSource.PlayOneShot(fallingSound);
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
    }
}
