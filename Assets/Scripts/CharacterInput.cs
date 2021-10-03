using UnityEngine;

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
    public float jumpPower = 5.0f;
    
    public Vector2 mouseSensitivity = new Vector2(1000, 800);
    
    [Header("Set in Prefab")]
    public Transform playerCameraParent;
    public PauseMenu pauseMenu;
    public Gun gun;
    

    private float _currentMaxSpeed = 0.0f;
    private Vector3 _velocity;
    private CharacterController _controller;
    // private Vector3 _groundNormal = Vector3.zero;
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _velocity = newVelocity;
    }

    bool IsRunning()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.IsPaused)
                pauseMenu.Unpause();
            else pauseMenu.Pause();
        }

        if (pauseMenu.IsPaused)
            return;
        
        Cursor.lockState = CursorLockMode.Locked;
        if (allowInput)
        {
            UpdateLookDirection();
            UpdateMovementDirection();
            UpdateJump();
            UpdateShoot();
        }
        ApplyGravity();

        // apply movement
        _controller.Move(_velocity);
    }

    private void UpdateJump()
    {
        if (CanJump() && Input.GetKey(KeyCode.Space))
            _velocity.y = jumpPower;
    }

    private void UpdateMovementDirection()
    {
        // get input
        var dir = Vector3.zero;
        _currentMaxSpeed = maxMoveSpeedStrafe;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            dir += Vector3.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            dir += Vector3.right;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            dir += Vector3.forward;
            _currentMaxSpeed = maxMoveSpeedForward;
            if (IsRunning())
                _currentMaxSpeed *= maxMoveSpeedRunModifier;
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
        
        // apply extra drag if no input
        if (dir == Vector3.zero && velocityXZ.magnitude > 0)
        {
            var dragThisFrame = dragOnNotMoving * GameTime.DeltaTime;
            if (velocityXZ.magnitude > dragThisFrame)
                velocityXZ -= (velocityXZ.normalized * dragThisFrame);
            else velocityXZ = Vector3.zero;
        }
        
        // clamp xy movement to maxspeed
        _currentMaxSpeed *= GameTime.DeltaTime;
        if (velocityXZ.magnitude > _currentMaxSpeed)
            velocityXZ = velocityXZ.normalized * _currentMaxSpeed;
        
        // apply back to velocity
        _velocity.x = velocityXZ.x;
        _velocity.z = velocityXZ.z;
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
        playerCameraParent.Rotate(-mouseSensitivity.y * (mouseY * GameTime.DeltaTime), 0.0f, 0.0f);
    }

    private void UpdateShoot()
    {
        if (Input.GetMouseButtonDown(0))
            gun.Shoot();
    }

    private bool CanJump()
    {
        var collisionFlags = _controller.collisionFlags;
        return _controller.isGrounded
               && (collisionFlags & CollisionFlags.Below) != 0
               && (collisionFlags & CollisionFlags.Sides) == 0;
    }

    private bool GroundIsTooSteep()
    {
        return (_controller.collisionFlags & CollisionFlags.Below) != 0; 
    }
}
