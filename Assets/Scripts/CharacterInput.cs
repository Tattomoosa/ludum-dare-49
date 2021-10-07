using System;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class CharacterInput : MonoBehaviour
{
    public bool allowInput = false;
    public float airAcceleration = 0.6f;
    public float groundAcceleration = 2.0f;
    
    public float maxMoveSpeedForward = 10.0f;
    public float maxMoveSpeedStrafe = 6.0f;
    public float maxMoveSpeedBack = 4.0f;
    public float maxMoveSpeedRunModifier = 1.5f;
    
    public float gravity = 9.8f;
    public float dragOnNotMoving = 3.0f;
    public float airDrag = 0.2f;
    public float jumpPower = 5.0f;
    public float slideSpeed = 1.0f;

    public Vector2 minMouseSensitivity = new Vector2(30, 30);
    public Vector2 maxMouseSensitivity = new Vector2(1000, 1000);
    [HideInInspector]
    public Vector2 mouseSensitivity;
    
    public UnityEvent<float> onInitMouseSensitivityX;
    public UnityEvent<float> onInitMouseSensitivityY;

    public UnityEvent onJump;

    // TODO for accessibility
    // public bool isSprintToggleSetting;
    // private bool _isSprinting;
    
    [Header("Set in Prefab")]
    public Transform playerCameraParent;
    public PauseMenu pauseMenu;
    public Gun gun;
    

    private float _currentMaxSpeed = 0.0f;
    private Vector3 _velocity;
    private CharacterController _controller;
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        mouseSensitivity = new Vector2(Screen.width / 4.0f, Screen.height / 4.0f);
        onInitMouseSensitivityX.Invoke(Mathf.InverseLerp(minMouseSensitivity.x, maxMouseSensitivity.x, mouseSensitivity.x));
        onInitMouseSensitivityY.Invoke(Mathf.InverseLerp(minMouseSensitivity.y, maxMouseSensitivity.y, mouseSensitivity.y));
        _controller.Move(Vector3.down);
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _velocity = newVelocity;
    }

    bool IsRunning()
    {
        
        return _controller.isGrounded
               && (Input.GetKey(KeyCode.LeftControl)
               || Input.GetKey(KeyCode.LeftShift)
               || Input.GetKey(KeyCode.RightShift)
               || Input.GetKey(KeyCode.RightControl));
    }
    
    void FixedUpdate()
    {
        if (GameTime.IsPaused || !allowInput) return;
        
        ApplyGravity();
        
        UpdateMovementDirection();
        UpdateJump();

        // apply movement
        _controller.Move(_velocity);
    }

    private void UpdateJump()
    {
        if (!CanJump() || !Input.GetKey(KeyCode.Space)) return;
        
        _velocity.y = jumpPower;
        onJump.Invoke();
    }

    private void UpdateMovementDirection()
    {
        // get input
        var dir = Vector3.zero;
        if (_controller.isGrounded)
            _currentMaxSpeed = maxMoveSpeedStrafe;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            dir += Vector3.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            dir += Vector3.right;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            dir += Vector3.forward;
            if (_controller.isGrounded)
            {
                _currentMaxSpeed = maxMoveSpeedForward;
                if (IsRunning())
                    _currentMaxSpeed *= maxMoveSpeedRunModifier;
            }
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            dir += Vector3.back;
            _currentMaxSpeed = maxMoveSpeedBack;
        }

        if (dir != Vector3.zero)
        {
            var speed = _controller.isGrounded
                ? groundAcceleration
                : airAcceleration;
            dir = transform.rotation * dir;
            // dir = playerCameraParent.rotation * dir;
            _velocity += dir * (speed * GameTime.DeltaTime);
        }
        
        // get horizontal direction
        var velocityXZ = new Vector3(_velocity.x, 0, _velocity.z);
        
        // apply drag if moving
        if (velocityXZ.magnitude > 0)
        {
            if (!_controller.isGrounded)
            {
                var airDragThisFrame = airDrag * GameTime.DeltaTime;
                if (velocityXZ.magnitude > airDragThisFrame)
                    velocityXZ -= (velocityXZ.normalized * airDragThisFrame);
                else velocityXZ = Vector3.zero;
            }
            // apply extra drag if no input
            if (dir == Vector3.zero)
            {
                var dragThisFrame = dragOnNotMoving * GameTime.DeltaTime;
                if (!_controller.isGrounded)
                    dragThisFrame /= 2.0f;
                if (velocityXZ.magnitude > dragThisFrame)
                    velocityXZ -= (velocityXZ.normalized * dragThisFrame);
                else velocityXZ = Vector3.zero;
            }

        }

        // clamp xy movement to maxspeed if on ground
        // if (_controller.isGrounded)
        // {
        var maxSpeed = _currentMaxSpeed * GameTime.DeltaTime;
        if (velocityXZ.magnitude > maxSpeed)
            velocityXZ = velocityXZ.normalized * maxSpeed;
        /*
        _currentMaxSpeed *= GameTime.DeltaTime;
        if (velocityXZ.magnitude > _currentMaxSpeed)
            velocityXZ = velocityXZ.normalized * _currentMaxSpeed;
            */
        // }

        // apply back to velocity
        _velocity.x = velocityXZ.x;
        _velocity.z = velocityXZ.z;
    }

    private void Update()
    {
        UpdatePause();
        
        if (GameTime.IsPaused || !allowInput) return;
        
        UpdateShoot();
        UpdateLookDirection();
    }

    private void UpdatePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            if (pauseMenu.IsPaused)
                pauseMenu.Unpause();
            else if (!GameTime.IsPaused)
                pauseMenu.Pause();
        }
    }

    private void ApplyGravity()
    {
        if (_velocity.y <= -gravity) return;
        
        if (_velocity.y > 0)
            _velocity.y -= (gravity * GameTime.DeltaTime) / 2;
        else
            _velocity.y -= gravity * GameTime.DeltaTime;
    }
    
    private void UpdateLookDirection()
    {
        if (!Application.isFocused)
            return;
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        
        transform.Rotate(0.0f, mouseSensitivity.x * (mouseX * GameTime.DeltaTime), 0.0f);
        
        // camera pitch
        
        float pitch = 0.0f;
        Vector3 axis = Vector3.right;
        playerCameraParent.transform.localRotation.ToAngleAxis(out pitch, out axis);
        if (axis.x < 0)
        {
            if (pitch > 80.0f && mouseY > 0.0f)
                mouseY = 0;
        }
        else
        {
            if (pitch > 80.0f && mouseY < 0.0f)
                mouseY = 0;
        }
        playerCameraParent.Rotate(-mouseSensitivity.y * (mouseY * GameTime.DeltaTime), 0.0f, 0.0f);
        // var playerCameraEuler = playerCameraParent.transform.localRotation.eulerAngles;
        // Debug.Log(playerCameraEuler.x);
        // if (playerCameraEuler.x < 80.0f)
        // playerCameraParent.transform.localRotation = Quaternion.Euler(new Vector3 (80.0f, playerCameraEuler.y, playerCameraEuler.z));
    }

    private void UpdateShoot()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (Input.GetMouseButtonDown(0))
            gun.Shoot();
    }
    

    private bool CanJump()
    {
        return _controller.isGrounded;
        /*
        var collisionFlags = _controller.collisionFlags;
        return _controller.isGrounded
               && (collisionFlags & CollisionFlags.Below) != 0
               && (collisionFlags & CollisionFlags.Sides) == 0;
               */
    }

        /*
         // attempt to make it so you can't jump up steep terrain.
         // works but feels terrible
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0.6)
            return;
        
        var tangent = Vector3.Cross(hit.normal, Vector3.up);
        var down = Vector3.Cross(hit.normal, tangent);
        _controller.enabled = false;
        // _controller.Move(down.normalized * (slideSpeed * Time.deltaTime));
        transform.position += (down.normalized * (slideSpeed * Time.deltaTime));
        _controller.enabled = true;
    }
        */

    public void SetMouseSensitivityX(float value)
    {
        mouseSensitivity.x = Mathf.Lerp(minMouseSensitivity.x, maxMouseSensitivity.x, value);
    }
    public void SetMouseSensitivityY(float value)
    {
        mouseSensitivity.y = Mathf.Lerp(minMouseSensitivity.y, maxMouseSensitivity.y, value);
    }
}
