using UnityEngine;

[RequireComponent(typeof(PlayerControls))]
public class PlayerCombats : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float fireAttackCooldown = 1f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform crouchFirePoint;
    [SerializeField] private GameObject fireballPrefab;
    
    private float _nextAttackTime = 0f;
    private float _nextFireAttackTime = 0f;
    private PlayerControls _controls;
    private Controls _input;
    private Animator _animator;
    private bool _attackInProgress;
    private bool _attackInputLock;

    private void Awake()
    {
        _controls = GetComponent<PlayerControls>();
        _animator = GetComponent<Animator>();
        _input = new Controls();
        _input.Enable();
    }

    private void Update()
    {
        if (_controls.IsDashing)
            return;

        HandleAttackInput();
        HandleFireAttackInput();
        ResetAttackInputLock();
    }

    private void HandleAttackInput()
    {
        var canAttack = Time.time >= _nextAttackTime && !_attackInProgress;

        if (_input.Player.Attack.triggered && canAttack)
        {
            StartAttack();
        }
    }

    private void HandleFireAttackInput()
    {
        if (_input.Player.FireAttack.triggered && Time.time >= _nextFireAttackTime)
        {
            PerformFireAttack();
        }
    }

    private void ResetAttackInputLock()
    {
        if (_input.Player.Attack.ReadValue<float>() == 0f)
        {
            _attackInputLock = false;
        }
    }

    private void StartAttack()
    {
        if (_attackInputLock) return;
        
        _attackInProgress = true;
        _attackInputLock = true;
        _nextAttackTime = Time.time + attackCooldown;
        _animator.SetInteger("Attack", 1);
    }
    
    public void DealDamage()
    {
       var hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
       
        foreach (var enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }
    
    public void EndAttack()
    {
        _animator.SetInteger("Attack", 0);
        _attackInProgress = false;
    }

    private void PerformFireAttack()
    {
        _nextFireAttackTime = Time.time + fireAttackCooldown;
        
        var currentFirePoint = _controls.IsCrouching ? crouchFirePoint : firePoint;
        Instantiate(fireballPrefab, currentFirePoint.position, currentFirePoint.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();
}