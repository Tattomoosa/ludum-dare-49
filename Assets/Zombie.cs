using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    private float gravity = 10.0f;
    private float speed = 2.0f;

    private CharacterController _controller;
    private Player _player;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        var velocity = Vector3.zero;

        velocity.y -= gravity * GameTime.DeltaTime;
        
        transform.LookAt(_player.transform.position);
        if (_controller.isGrounded)
            velocity += transform.forward * (speed * GameTime.DeltaTime);

        _controller.Move(velocity);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}