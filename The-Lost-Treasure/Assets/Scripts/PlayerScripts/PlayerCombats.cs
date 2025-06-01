using UnityEngine;

public class PlayerCombats : SoundEmitter
{
    [Header("Combat Settings")]
    [SerializeField] private float swordAttackRange = 2f;
    [SerializeField] private float swordAttackDamage = 20f;
    [SerializeField] private float swordAttackCooldown = 1f;
    [SerializeField] private float staffAttackRange = 1.5f;
    [SerializeField] private float staffAttackDamage = 15f;
    [SerializeField] private float staffAttackCooldown = 0.7f;
    [SerializeField] private float fireAttackCooldown = 1f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform crouchFirePoint;
    [SerializeField] private GameObject fireballPrefab;
    
    private float _nextAttackTime = 0f;
    private float _nextSpecialAttackTime = 0f;
    private PlayerControls _controls;
    private Controls _input;
    private Animator _animator;
    private bool _attackInProgress;
    private bool _attackInputLock;
    private float _fireAttackDelay = 0.6f;
    
    private IGun _currentWeapon;
    private SwordWeapon _swordWeapon;
    private StaffWeapon _staffWeapon;

    private void Awake()
    {
        _controls = GetComponent<PlayerControls>();
        _animator = GetComponent<Animator>();
        _input = new Controls();
        _input.Enable();
        
        _swordWeapon = new SwordWeapon(swordAttackDamage, swordAttackRange, swordAttackCooldown);
        _staffWeapon = new StaffWeapon(staffAttackDamage, staffAttackRange, staffAttackCooldown, fireAttackCooldown, fireballPrefab);
        
        SetWeapon(_controls.GunType);
    }

    private void Update()
    {
        if (_controls.IsDashing)
            return;

        HandleWeaponSwitch();
        HandleAttackInput();
        HandleSpecialAttackInput();
        ResetAttackInputLock();
    }

    private void HandleWeaponSwitch()
    {
        if (_input.Player.SwitchWeapon1.triggered || _input.Player.SwitchWeapon2.triggered)
        {
            var newWeaponType = _controls.GunType;
            SetWeapon(newWeaponType);
        }
    }

    private void SetWeapon(int weaponType)
    {
        _currentWeapon = weaponType == 0 ? _staffWeapon : _swordWeapon;
    }

    private void HandleAttackInput()
    {
        var canAttack = Time.time >= _nextAttackTime && !_attackInProgress;

        if (_input.Player.Attack.triggered && canAttack)
        {
            StartAttack();
        }
    }

    private void HandleSpecialAttackInput()
    {
        if (_input.Player.FireAttack.triggered && Time.time >= _nextSpecialAttackTime)
        {
            PerformSpecialAttack();
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
        _nextAttackTime = Time.time + _currentWeapon.GetAttackCooldown();
        _currentWeapon.PerformAttack(this);
    }

    // Этот метод вызывается из анимации
    public void DealDamage()
    {
        var hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, _currentWeapon.Range, enemyLayers);
       
        foreach (var enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(_currentWeapon.Damage);
            }
        }
    }

    // Этот метод вызывается из анимации
    public void DealSpecialDamage()
    {
        var hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, _currentWeapon.SpecialRange, enemyLayers);
       
        foreach (var enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(_currentWeapon.SpecialDamage);
            }
        }
    }
    
    public void EndAttack()
    {
        _animator.SetInteger("Attack", 0);
        _attackInProgress = false;
    }

    private void PerformSpecialAttack()
    {
        _nextSpecialAttackTime = Time.time + _currentWeapon.GetSpecialAttackCooldown();
        _currentWeapon.PerformSpecialAttack(this);
    }

    public void PlayAttackSound()
    {
        var sound =  _currentWeapon is StaffWeapon ? sounds[0] : sounds[2];
        PlaySound(sound);
    }
    
    public void PlaySpecialAttackSound()
    {
        var sound =  _currentWeapon is StaffWeapon ? sounds[1] : sounds[3];
        PlaySound(sound);
    }
    
    public Transform GetFirePoint(bool isCrouching)
    {
        return isCrouching ? crouchFirePoint : firePoint;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, _currentWeapon?.Range ?? swordAttackRange);
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();
}