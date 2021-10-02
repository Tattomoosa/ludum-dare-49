using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterInput : MonoBehaviour
{
    public bool allowInput = false;
    public float airAcceleration = 0.6f;
    public float groundAcceleration = 2.0f;
    
    public float maxMoveSpeed = 10.0f;
    public float maxMoveSpeedRunModifier = 1.5f;
    
    public float gravity = 9.8f;
    public float dragOnNotMoving = 3.0f;
    public float jumpPower = 5.0f;
    
    public Vector2 mouseSensitivity = new Vector2(1000, 800);
    
    
    [Header("Set in Prefab")]
    public Transform playerCameraParent;
    public PauseMenu pauseMenu;
    public Gun gun;

    private Vector3 velocity;
    
    
    private CharacterController _controller;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    bool IsRunning()
    {
        return Input.GetKey(KeyCode.LeftControl);
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
        _controller.Move(velocity);
    }

    private void UpdateJump()
    {
        if (_controller.isGrounded && Input.GetKey(KeyCode.Space))
            velocity.y = jumpPower;
    }

    private void UpdateMovementDirection()
    {
        // get input
        var dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            dir += Vector3.forward;
        if (Input.GetKey(KeyCode.A))
            dir += Vector3.left;
        if (Input.GetKey(KeyCode.S))
            dir += Vector3.back;
        if (Input.GetKey(KeyCode.D))
            dir += Vector3.right;

        if (dir != Vector3.zero)
        {
            var speed = _controller.isGrounded
                ? groundAcceleration
                : airAcceleration;
            dir = transform.rotation * dir;
            // dir = playerCameraParent.rotation * dir;
            velocity += dir * (speed * GameTime.DeltaTime);
        }
        
        // get horizontal direction
        var velocityXZ = new Vector3(velocity.x, 0, velocity.z);
        
        // apply extra drag if no input
        if (dir == Vector3.zero && velocityXZ.magnitude > 0)
        {
            var dragThisFrame = dragOnNotMoving * GameTime.DeltaTime;
            if (velocityXZ.magnitude > dragThisFrame)
                velocityXZ -= (velocityXZ.normalized * dragThisFrame);
            else velocityXZ = Vector3.zero;
        }
        
        // clamp xy movement to maxspeed
        var max = maxMoveSpeed * GameTime.DeltaTime;
        if (IsRunning())
            max *= maxMoveSpeedRunModifier;
        if (velocityXZ.magnitude > max)
            velocityXZ = velocityXZ.normalized * max;
        
        // apply back to velocity
        velocity.x = velocityXZ.x;
        velocity.z = velocityXZ.z;
    }

    private void ApplyGravity()
    {
        // apply gravity
        if (velocity.y <= -gravity) return;
        
        if (velocity.y > 0)
            velocity.y -= (gravity * GameTime.DeltaTime) / 2;
        else
            velocity.y -= gravity * GameTime.DeltaTime;
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
}
