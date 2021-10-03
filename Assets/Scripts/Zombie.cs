using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Zombie : MonoBehaviour
{
    public UnityEvent onAwareOfPlayer;
    public float waitBeforeExplode = 0.5f;
    [SerializeField]
    private float triggerExplodeDistance = 2.0f;
    [SerializeField]
    private float actualExplodeHitDistance = 4.0f;
    [SerializeField]
    private float gravity = 10.0f;
    [SerializeField]
    private float speed = 2.0f;

    private float deathFallSpeed = 1.5f;
    
    private bool _isDead = false;

    [SerializeField] private Collider headCollider;
    
    private AudioSource _audioSource;
    private Animator _animator;
    private CharacterController _controller;
    private static Player _player;
    private static readonly int DieAnimation = Animator.StringToHash("Die");
    private static readonly int MoveSpeedAnimation = Animator.StringToHash("MoveSpeed");

    private bool _isExploding = false;
    private bool _isActive = false;
    private static readonly int ExplodeAnimation = Animator.StringToHash("Explode");

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _controller = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        if (!_player)
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

        var distanceToPlayer = GetDistanceToPlayer();
        
        if (_isExploding)
            return;
        
        if (distanceToPlayer < triggerExplodeDistance)
            Explode();
        
        var velocity = Vector3.zero;

        velocity.y -= gravity * GameTime.DeltaTime;

        var lookPosition = _player.transform.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);
        if (_controller.isGrounded)
            velocity += transform.forward * (speed * GameTime.DeltaTime);

        var velocityXZ = new Vector3(velocity.x, 0, velocity.z);
        _animator.SetFloat(MoveSpeedAnimation, velocityXZ.magnitude);
        _controller.Move(velocity);
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _player.transform.position);
    }

    private void Explode()
    {
        _isExploding = true;
        _animator.SetTrigger(ExplodeAnimation);
        // todo explosion particles
        StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(waitBeforeExplode);
        // spawn explosion particles
        var distanceToPlayer = GetDistanceToPlayer();
        if (distanceToPlayer < actualExplodeHitDistance)
            _player.Die();
        Die();
    }

    public void Die()
    {
        if (!_isDead)
            StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        _audioSource.Stop();
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