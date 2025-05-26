using System.Collections;
using UnityEngine;

public class StaffWeapon : MonoBehaviour, IGun
{
    public float Damage { get; }
    public float Range { get; }
    public float SpecialDamage => 0; 
    public float SpecialRange => 0;
    private readonly float _cooldown;
    private readonly float _specialCooldown;
    private readonly GameObject _fireballPrefab;
    private readonly float _fireAttackDelay = 0.6f;

    public StaffWeapon(float damage, float range, float cooldown, float specialCooldown, GameObject fireballPrefab)
    {
        Damage = damage;
        Range = range;
        _cooldown = cooldown;
        _specialCooldown = specialCooldown;
        _fireballPrefab = fireballPrefab;
    }

    public void PerformAttack(PlayerCombats playerCombats)
    {
        playerCombats.PlayAttackSound();
        playerCombats.GetComponent<Animator>().SetInteger("Attack", 1); 
    }

    public void PerformSpecialAttack(PlayerCombats playerCombats)
    {
        playerCombats.PlaySpecialAttackSound();
        playerCombats.StartCoroutine(FireAfterDelay(playerCombats));
    }

    private IEnumerator FireAfterDelay(PlayerCombats playerCombats)
    {
        yield return new WaitForSeconds(_fireAttackDelay);

        var firePoint = playerCombats.GetFirePoint(playerCombats.GetComponent<PlayerControls>().IsCrouching);
        Instantiate(_fireballPrefab, firePoint.position, firePoint.rotation);
    }

    public float GetAttackCooldown() => _cooldown;
    public float GetSpecialAttackCooldown() => _specialCooldown;
}