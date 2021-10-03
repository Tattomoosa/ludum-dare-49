using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Zombie : MonoBehaviour
{
    public UnityEvent onAwareOfPlayer;
    [SerializeField]
    private float gravity = 10.0f;
    [SerializeField]
    private float speed = 2.0f;

    private float deathFallSpeed = 1.5f;
    
    private bool _isDead = false;

    private Animator _animator;
    private CharacterController _controller;
    private Player _player;
    private static readonly int DieAnimation = Animator.StringToHash("Die");
    private static readonly int MoveSpeedAnimation = Animator.StringToHash("MoveSpeed");

    private bool _isActive = false;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _controller = GetComponent<CharacterController>();
        _player = FindObjectOfType<Player>();
    }

    public void SetActive(bool value)
    {
        _isActive = value;
        if (_isActive)
            onAwareOfPlayer.Invoke();
    }

    private void Update()
    {
        if (!_isActive || _isDead)
            return;
        
        var velocity = Vector3.zero;

        velocity.y -= gravity * GameTime.DeltaTime;
        
        transform.LookAt(_player.transform.position);
        if (_controller.isGrounded)
            velocity += transform.forward * (speed * GameTime.DeltaTime);

        var velocityXZ = new Vector3(velocity.x, 0, velocity.z);
        _animator.SetFloat(MoveSpeedAnimation, velocityXZ.magnitude);
        _controller.Move(velocity);
    }

    public void Die()
    {
        if (!_isDead)
            StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        _isDead = true;
        _animator.SetTrigger(DieAnimation);
        yield return new WaitForSeconds(2.0f);
        var fallPosition = transform.position - (Vector3.up * 4.0f);
        while (transform.position.y > fallPosition.y)
        {
            transform.position -= Vector3.up * (deathFallSpeed * GameTime.DeltaTime);
            yield return 0;
        }
        Destroy(gameObject);
    }
}