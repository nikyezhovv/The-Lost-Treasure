using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    public float speed = 4f;

    [SerializeField] public float sprintMultiplier = 1.5f;

    [Header("JumpSettings")] [SerializeField]
    public float jumpForce = 3f;

    [SerializeField] public float airControlFactor = 0.7f;

    [Header("Ground Check")] [SerializeField]
    public Transform groundCheck;

    [SerializeField] public float groundRadius = 0.2f;
    [SerializeField] public LayerMask layerGrounds;

    private const int MaxJumps = 2;

    private Rigidbody2D _rigidbody;
    private Controls _input;
    private SpriteRenderer _sprite;

    private bool _isSprinting;
    private bool _isGrounded;
    private float _movementX;
    private float _activeMoveSpeed;
    private int _jumpCount;
    private float _jumpStartSpeed;

    private void Awake()
    {
        _input = new Controls();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _sprite = gameObject.GetComponentInChildren<SpriteRenderer>();

        SetupInputCallbacks();
    }

    private void SetupInputCallbacks()
    {
        _input.Player.Move.performed += context => Move(context.ReadValue<float>());
        _input.Player.Move.canceled += _ => Move(0);
        _input.Player.Jump.performed += _ => Jump();
        _input.Player.Sprint.performed += _ => _isSprinting = true;
        _input.Player.Sprint.canceled += _ => _isSprinting = false;
    }

    private void Update()
    {
        _rigidbody.linearVelocity = new Vector2(_activeMoveSpeed, _rigidbody.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        UpdateGroundedStatus();
        UpdateMovementSpeed();
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
        return speed * (_isSprinting ? sprintMultiplier : 1f);
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();
}