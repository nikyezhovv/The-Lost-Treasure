using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerControls : Sounds
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
    [SerializeField] private bool isEnter = false;

    [Header("Map bounds settings")]
    [SerializeField] private float deathY = -6f;
    
    public bool isGrounded;
    
    private const int MaxJumps = 2;

    public Animator anim;
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
    
    public bool IsDashing => _isDashing;
    public bool IsCrouching => _isCrouching;

    private void Awake()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        anim = GetComponent<Animator>();
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

    private void UpdateGroundedStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, layerGrounds);

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
            anim.SetInteger("State", 1);
            _sprite.rotation = Quaternion.Euler(0, axis > 0 ? 0 : 180, 0);
        }
        else
            anim.SetInteger("State", 0);
    }

    private void Jump()
    {
        if (isGrounded || _jumpCount < MaxJumps - 1)
        {
            _fallThroughButtonHeld = true;
            anim.SetBool("isJump", true); //Start jump animation
            anim.SetBool("isDown", false);
            PlaySound(sounds[1]);
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, jumpForce);
            _jumpStartSpeed = GetCurrentSpeed();
            _jumpCount++;
        }
    }

    [System.Obsolete]
    private void UpdateFallingStatus()  
    {
        if (!isGrounded && _rigidbody.linearVelocity.y < 0)
        {
            if (!isEnter)
                _fallThroughButtonHeld = false;
            anim.SetBool("isJump", false);
            anim.SetBool("isDown", true);
        }
        else if (isGrounded)
        {
            anim.SetBool("isDown", false);
            // Reset when grounded
        }
    }

    //Вызывается из аниматора
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
        _input.Disable();
        //anim.SetBool("isDown", true);
        _rigidbody.linearVelocity = Vector2.zero;


        yield return new WaitForSeconds(duration);
        
        //anim.SetBool("isDown", false);
        _input.Enable();
    }

    private void Dash()
    {
        if (!isGrounded) return;

        if (_canDash && !_isCrouching)
        {
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
    }
    
    private void StopDash()
    {
        anim.SetBool("isDown", true);
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
        anim.SetBool("isCreep", true);
        _isCrouching = true;
        standingCollider.enabled = false;
        crouchingCollider.enabled = true;
    }

    private void StopCrouch()
    {
        anim.SetBool("isCreep", false);
        _isCrouching = false;
        crouchingCollider.enabled = false;
        standingCollider.enabled = true;
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform") && !_currentPlatformColliders.Contains(collision))
        {
            isEnter = true;
            _currentPlatformColliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            isEnter = false;
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