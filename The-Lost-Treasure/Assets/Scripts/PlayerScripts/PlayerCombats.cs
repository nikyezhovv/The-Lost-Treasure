using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerControls))]
public class PlayerCombats : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float fireAttackCooldown = 1f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform crouchFirePoint;
    [SerializeField] private GameObject fireballPrefab;
    
    private float _lastAttackTime;
    private float _lastFireAttackTime;
    private PlayerControls _controls;
    private Controls _input;
    private bool _canAttack = true;
    private bool _canFireAttack = true;
    private Animator _animator;

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

        if (_input.Player.Attack.triggered && _canAttack)
        {
            StartCoroutine(PerformAttack());
        }

        if (_input.Player.FireAttack.triggered && _canFireAttack)
        {
            StartCoroutine(PerformFireAttack());
        }
    }

    private IEnumerator PerformFireAttack()
    {
        _canFireAttack = false;
        _lastFireAttackTime = Time.time;
        
        
        var currentFirePoint = _controls.IsCrouching ? crouchFirePoint : firePoint;
        Instantiate(fireballPrefab, currentFirePoint.position, currentFirePoint.rotation);
        
        yield return new WaitForSeconds(fireAttackCooldown);
        _canFireAttack = true;
    }
    
    private IEnumerator PerformAttack()
    {
        _canAttack = false;
        _lastAttackTime = Time.time;
        _animator.SetInteger("Attack", 1);
        
        yield return new WaitForSeconds(0.1f);
        
        var hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
       
        foreach (var enemy in hitEnemies)
        {
            if (enemy.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(attackCooldown - 0.1f);
        _canAttack = true;
        _animator.SetInteger("Attack", 0);
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