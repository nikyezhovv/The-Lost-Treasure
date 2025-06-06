using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControls : SoundEmitter
{
    [Header("Gun Swap")]
    [SerializeField] private bool canSwapGun;
    
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
    [SerializeField] private bool isEnter = false;

    [Header("Map bounds settings")]
    [SerializeField] private float deathY = -6f;
    
    public bool isGrounded;
    
    private const int MaxJumps = 2;

    public Animator animator;
    private Rigidbody2D _rigidbody;
    private Controls _input;
    private Transform _sprite;
    private bool _isSprinting;
    private bool _canDash = true;
    private bool _isDashing;
    private bool _isCrouching;
    private bool _wasGrounded;
    private PlayerHealth _playerHealth;
    private bool _isFallingThroughPlatform;
    private float _fallThroughTimer;
    private List<Collider2D> _currentPlatformColliders = new ();
    private bool _fallThroughButtonHeld;
    private float _movementX;
    private float _activeMoveSpeed;
    private int _jumpCount;
    private float _jumpStartSpeed;
    private float _defaultGravityScale;
    private float _poisonSlowMultiplier = 1f;
    private int _gunType;
    
    public bool IsDashing => _isDashing;
    public bool IsCrouching => _isCrouching;

    public int GunType => _gunType;

    private void Awake()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();
        _input = new Controls();
        _rigidbody = GetComponent<Rigidbody2D>();
        _defaultGravityScale = _rigidbody.gravityScale;
        _sprite = GetComponentInChildren<Transform>();

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
        _input.Player.FallThrough.performed += _ => _fallThroughButtonHeld = true;
        _input.Player.FallThrough.canceled += _ => _fallThroughButtonHeld = false;
        _input.Player.SwitchWeapon1.performed += _ => SwitchWeapon(0);
        _input.Player.SwitchWeapon2.performed += _ => SwitchWeapon(1);
    }
    
    private void FixedUpdate()
    {
        if (transform.position.y < deathY)
        {
            PlaySound(sounds[3]);
            _playerHealth.Respawn();
        } 
        
        var wasGrounded = isGrounded;
        UpdateGroundedStatus();
        
        if (!wasGrounded && isGrounded)
        {
            PlayLandingSound();
        }
        
        _wasGrounded = isGrounded;
        
        UpdateMovementSpeed();
        HandlePlatformCollisions();
        UpdateFallingStatus();

        if (!_isDashing)
        {
            _rigidbody.linearVelocity = new Vector2(_activeMoveSpeed, _rigidbody.linearVelocity.y);
        }
    }

    private void PlayLandingSound()
    {
        PlaySound(sounds[2]);
    }
    
    private void SwitchWeapon(int weaponIndex)
    {
        if (!canSwapGun || _gunType == weaponIndex) return;
        animator.SetInteger("GunType", weaponIndex);
        _gunType = weaponIndex;
        PlaySound(sounds[4]);
        Debug.Log("Switched to weapon: " + weaponIndex);
    }

    private void UpdateGroundedStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, layerGrounds);
        
        if (isGrounded != _wasGrounded)
        {
            animator.SetBool("IsGrounded", isGrounded);
        }
        
        if (isGrounded)
        {
            _jumpCount = 0;
        }
    }

    private void UpdateMovementSpeed()
    {
        if (isGrounded)
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
            animator.SetInteger("State", 1);
            _sprite.rotation = Quaternion.Euler(0, axis > 0 ? 0 : 180, 0);
        }
        else
        {
            animator.SetInteger("State", 0);
        }
    }

    private void Jump()
    {
        if (!isGrounded && _jumpCount >= MaxJumps - 1) return;
        
        _fallThroughButtonHeld = true;
        animator.SetBool("isJump", true);
        animator.SetBool("isDown", false);
        PlaySound(sounds[1]);
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, jumpForce);
        _jumpStartSpeed = GetCurrentSpeed();
        _jumpCount++;
    }
    
    private void UpdateFallingStatus()  
    {
        if (!isGrounded && _rigidbody.linearVelocity.y < 0)
        {
            if (!isEnter)
            {
                _fallThroughButtonHeld = false;
            }
                
            animator.SetBool("isJump", false);
            animator.SetBool("isDown", true);
        }
        else if (isGrounded)
        {
            animator.SetBool("isDown", false);
        }
    }

    private void PlayWalkSound()
    {
        PlaySound(sounds[0]);
    }

    private float GetCurrentSpeed()
    {
        var baseSpeed = speed;

        if (_isCrouching)
        {
            baseSpeed *= crouchMultiplier;
        }
        else if (_isSprinting)
        {
            baseSpeed *= sprintMultiplier;
        }

        return baseSpeed * _poisonSlowMultiplier;
    }

    public void Stun(float stunTime)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(StunCoroutine(stunTime));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        animator.SetBool("IsStun", true);
        _input.Disable();
        _rigidbody.linearVelocity = Vector2.zero;
        
        yield return new WaitForSeconds(duration);
        animator.SetBool("IsStun", false);
        _input.Enable();
    }

    private void Dash()
    {
        if (!isGrounded || !_canDash || _isCrouching) return;
        
        var currentRotationY = _sprite.rotation.eulerAngles.y;

        var dashDirection = new Vector2((currentRotationY > 90) ? -1 : 1, 0);
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.gravityScale = 0f;
        _rigidbody.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);

        _canDash = false;
        _isDashing = true;

        Invoke(nameof(StopDash), dashDuration);
        Invoke(nameof(ResetDash), dashCooldown);
    }
    
    private void StopDash()
    {
        animator.SetBool("isDown", true);
        _isDashing = false;
        _rigidbody.gravityScale = _defaultGravityScale;
    }
    
    public void ApplyPoisonSlow(float slowMultiplier)
    {
        _poisonSlowMultiplier = slowMultiplier;
    }

    public void ClearPoisonSlow()
    {
        _poisonSlowMultiplier = 1f;
    }

    private void ResetDash()
    {
        _canDash = true;
    }

    private void StartCrouch()
    {
        animator.SetBool("isCreep", true);
        _isCrouching = true;
        standingCollider.enabled = false;
        crouchingCollider.enabled = true;
    }

    private void StopCrouch()
    {
        animator.SetBool("isCreep", false);
        _isCrouching = false;
        crouchingCollider.enabled = false;
        standingCollider.enabled = true;
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Platform") || _currentPlatformColliders.Contains(collision)) return;
        isEnter = true;
        _currentPlatformColliders.Add(collision);
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Platform")) return;
        isEnter = false;
        _currentPlatformColliders.Remove(collision);
        _fallThroughButtonHeld = false;
    }

    private void HandlePlatformCollisions()
    {
        foreach (var platformCollider in _currentPlatformColliders)
        {
            if (platformCollider != null && platformCollider.CompareTag("Platform"))
            {
                Physics2D.IgnoreCollision(standingCollider, platformCollider, _fallThroughButtonHeld);
                Physics2D.IgnoreCollision(crouchingCollider, platformCollider, _fallThroughButtonHeld);
            }
        }
    }
}