using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float speed = 4f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float crouchMultiplier = 0.5f; 

    [Header("Jump Settings")] 
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float airControlFactor = 0.7f;

    [Header("Ground Check")] 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask layerGrounds;

    [Header("Dash Settings")] 
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashDuration = 0.5f;
    
    [Header("Colliders")]
    [SerializeField] private Collider2D standingCollider;
    [SerializeField] private Collider2D crouchingCollider;

    private const int MaxJumps = 2;

    private Rigidbody2D _rigidbody;
    private Controls _input;
    private SpriteRenderer _sprite;

    private bool _isSprinting;
    private bool _isGrounded;
    private bool _canDash = true;
    private bool _isDashing;
    private bool _isCrouching;

    private float _movementX;
    private float _activeMoveSpeed;
    private int _jumpCount;
    private float _jumpStartSpeed;
    private float _defaultGravityScale;
    
    public SpriteRenderer PlayerSprite => _sprite;
    public bool IsFacingLeft => _sprite.flipX;
    public bool IsDashing => _isDashing;
    public bool IsCrouching => _isCrouching;

    private void Awake()
    {
        _input = new Controls();
        _rigidbody = GetComponent<Rigidbody2D>();
        _defaultGravityScale = _rigidbody.gravityScale; 
        _sprite = GetComponentInChildren<SpriteRenderer>();

        SetupInputCallbacks();
    }

    private void SetupInputCallbacks()
    {
        _input.Player.Move.performed += context => Move(context.ReadValue<float>());
        _input.Player.Move.canceled += _ => Move(0);
        _input.Player.Jump.performed += _ => Jump();
        _input.Player.Sprint.performed += _ => _isSprinting = true;
        _input.Player.Sprint.canceled += _ => _isSprinting = false;
        _input.Player.Dash.performed += _ => Dash();
        _input.Player.Crouch.performed += _ => StartCrouch();
        _input.Player.Crouch.canceled += _ => StopCrouch();
    }

    private void FixedUpdate()
    {
        UpdateGroundedStatus();
        UpdateMovementSpeed();

        if (!_isDashing)
        {
            _rigidbody.linearVelocity = new Vector2(_activeMoveSpeed, _rigidbody.linearVelocity.y);
        }
    }

    private void UpdateGroundedStatus()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, layerGrounds);

        if (_isGrounded)
        {
            _jumpCount = 0;
        }
    }

    private void UpdateMovementSpeed()
    {
        if (_isGrounded)
        {
            _activeMoveSpeed = _movementX * GetCurrentSpeed();
        }
        else
        {
            _activeMoveSpeed = _movementX * _jumpStartSpeed * airControlFactor;
        }
    }

    private void Move(float axis)
    {
        _movementX = axis;
        if (axis != 0)
        {
            _sprite.flipX = axis < 0;
        }
    }

    private void Jump()
    {
        if (_isGrounded || _jumpCount < MaxJumps - 1)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, jumpForce);
            _jumpStartSpeed = GetCurrentSpeed();
            _jumpCount++;
        }
    }

    private float GetCurrentSpeed()
    {
        float baseSpeed = speed;

        if (_isCrouching)
        {
            baseSpeed *= crouchMultiplier;
        }
        else if (_isSprinting)
        {
            baseSpeed *= sprintMultiplier;
        }

        return baseSpeed;
    }

    private void Dash()
    {
        if (_canDash && !_isCrouching)
        {
            var dashDirection = new Vector2(_sprite.flipX ? -1 : 1, 0);
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.gravityScale = 0f;
            _rigidbody.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);

            _canDash = false;
            _isDashing = true;

            Invoke(nameof(StopDash), dashDuration);
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    private void StopDash()
    {
        _isDashing = false;
        _rigidbody.gravityScale = _defaultGravityScale;
    }

    private void ResetDash()
    {
        _canDash = true;
    }

    private void StartCrouch()
    {
        _isCrouching = true;
        standingCollider.enabled = false;
        crouchingCollider.enabled = true;
        transform.localScale = new Vector3(1f, 0.5f, 1f);
    }

    private void StopCrouch()
    {
        _isCrouching = false;
        crouchingCollider.enabled = false;
        standingCollider.enabled = true;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();
}