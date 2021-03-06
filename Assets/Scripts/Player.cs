using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Checkpoint lastCheckpoint;

    [Header("Set in Player Prefab")]
    
    public FadeInOut fader;

    public CheckpointGetUI checkpointGetUI;

    private CharacterController _controller;
    private CharacterInput _input;

    public bool IsDead { get; private set; } = false;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<CharacterInput>();

        _input.allowInput = false;
        fader.gameObject.SetActive(true);
        StartCoroutine(FadeInCoroutine());
    }
    
    public void Die()
    {
        Respawn();
    }

    public Coroutine Respawn()
    {
        return StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        IsDead = true;
        _input.enabled = false;
        _input.allowInput = false;
        yield return fader.FadeIn();
        yield return new WaitForSeconds(1.0f);
        SetPosition(lastCheckpoint.transform.position);
        transform.rotation = lastCheckpoint.transform.rotation;
        _controller.Move(Vector3.down * 100.0f);
        _input.enabled = true;
        _input.SetVelocity(Vector3.zero);
        yield return fader.FadeOut();
        _input.allowInput = true;
        IsDead = false;
    }

    private IEnumerator FadeInCoroutine()
    {
        fader.InstantFadeIn();
        yield return fader.FadeOut();
        _input.allowInput = true;
    }

    public void SetPosition(Vector3 position)
    {
        _controller.enabled = false;
        transform.position = position;
        _controller.enabled = true;
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        if (lastCheckpoint == checkpoint)
            return;
        lastCheckpoint = checkpoint;
        checkpointGetUI.Show();
    }
}

