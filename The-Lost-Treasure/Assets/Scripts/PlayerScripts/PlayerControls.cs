using System.Collections.Generic;
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

    [Header("Fall Through Platform")]
    [SerializeField] private float fallThroughDelay = 0.2f;  //*****


    private const int MaxJumps = 2;

    private Rigidbody2D _rigidbody;
    private Controls _input;
    private SpriteRenderer _sprite;

    private bool _isSprinting;
    private bool _isGrounded;
    private bool _canDash = true;
    private bool _isDashing;
    private bool _isCrouching;

    private bool _isFallingThroughPlatform;  // Added flag
    private float _fallThroughTimer;  //Added timer

    // New Variables
    private List<Collider2D> _currentPlatformColliders = new List<Collider2D>();
    private bool _fallThroughButtonHeld;

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

        //  _input.Player.FallThrough.performed += _ => StartFallThrough();
        _input.Player.FallThrough.performed += _ => _fallThroughButtonHeld = true;
      //  _input.Player.FallThrough.canceled += _ => _fallThroughButtonHeld = false;
    }

    private void FixedUpdate()
    {
        UpdateGroundedStatus();
        UpdateMovementSpeed();
        HandlePlatformCollisions();

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



    //private void UpdateFallThroughPlatform()
    //{
    //    if (_isFallingThroughPlatform)
    //    {
    //        _fallThroughTimer -= Time.fixedDeltaTime;
    //        if (_fallThroughTimer <= 0f)
    //        {
    //            _isFallingThroughPlatform = false;
    //        }
    //    }
    //}

    //private void StartFallThrough()
    //{
    //    if (_isGrounded)
    //    {
    //        _isFallingThroughPlatform = true;
    //        _fallThroughTimer = fallThroughDelay;
    //        // Optionally, you could disable the collider of the platform you're standing on directly.  However,
    //        // that would require knowing which specific platform you're standing on. This method is usually more robust.
    //        IgnorePlatformCollisions();  // Call the method to ignore collisions with platforms
    //    }
    //}

    //private void IgnorePlatformCollisions()
    //{
    //    // Perform a sphere cast to find the platform we're standing on
    //    RaycastHit2D hit = Physics2D.CircleCast(groundCheck.position, groundRadius, Vector2.down, 0.1f, layerGrounds);

    //    if (hit.collider != null && hit.collider.CompareTag("Platform"))
    //    {
    //        // Get the platform collider and ignore collisions between the player collider and the platform collider
    //        Collider2D platformCollider = hit.collider;
    //        Physics2D.IgnoreCollision(standingCollider, platformCollider, true);
    //        Physics2D.IgnoreCollision(crouchingCollider, platformCollider, true);
    //        //Start a coroutine to re-enable the collisions after a delay
    //        StartCoroutine(ReEnablePlatformCollisions(platformCollider, fallThroughDelay));
    //    }
    //}

    //private System.Collections.IEnumerator ReEnablePlatformCollisions(Collider2D platformCollider, float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    if (platformCollider != null)
    //    {
    //        Physics2D.IgnoreCollision(standingCollider, platformCollider, false);
    //        Physics2D.IgnoreCollision(crouchingCollider, platformCollider, false);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform") && !_currentPlatformColliders.Contains(collision))
        {
            _currentPlatformColliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            _currentPlatformColliders.Remove(collision);
            _fallThroughButtonHeld = false;
        }
    }

    private void HandlePlatformCollisions()
    {
        foreach (Collider2D platformCollider in _currentPlatformColliders)
        {
            if (platformCollider != null && platformCollider.CompareTag("Platform"))
            {
                Physics2D.IgnoreCollision(standingCollider, platformCollider, _fallThroughButtonHeld);
                Physics2D.IgnoreCollision(crouchingCollider, platformCollider, _fallThroughButtonHeld);
            }
        }
    }
}