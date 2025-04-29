using UnityEngine;
using System.Collections;
using UnityEngine.Windows;

[RequireComponent(typeof(PlayerControls))]
public class PlayerCombats : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    
    private float _lastAttackTime;
    private PlayerControls _controls;
    private Controls _input;
    private bool _canAttack = true;

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
        if (_controls.IsDashing || _controls.IsCrouching)
            return;

        if (_input.Player.Attack.triggered && _canAttack)
        {
            StartCoroutine(PerformAttack());
        }
        //    if (_canAttack)
        //      _input.Player.Attack.performed += _ => StartCoroutine(PerformAttack());
        //    _input.Player.Attack.canceled += _ => _animator.SetInteger("Attack", 0);

    }
    private void SetupInputCallbacks()
    {
    }

        private IEnumerator PerformAttack()
    {
        _canAttack = false;
        _lastAttackTime = Time.time;
        _animator.SetInteger("Attack", 1);
        Debug.Log("Player starts attack");
        
        yield return new WaitForSeconds(0.1f);
        
        var hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
       
        if (hitEnemies.Length == 0)
        {
            Debug.Log("No enemies hit.");
        }
        else
        {
            
            foreach (var enemy in hitEnemies)

            {
                Debug.Log($"Hit enemy: {enemy.gameObject.name}, dealing {attackDamage} damage");

                if (enemy.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(attackDamage);
                }
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
