using UnityEngine;
public class SwordWeapon : MonoBehaviour, IGun
{
    public float Damage { get; }
    public float Range { get; }
    public float SpecialDamage { get; }
    public float SpecialRange { get; }
    private readonly float _cooldown;

    public SwordWeapon(float damage, float range, float cooldown)
    {
        Damage = damage;
        Range = range;
        SpecialDamage = damage * 1.5f;
        SpecialRange = range * 1.5f;
        _cooldown = cooldown;
    }

    public void PerformAttack(PlayerCombats playerCombats)
    {
        playerCombats.PlayAttackSound();
        playerCombats.GetComponent<Animator>().SetInteger("Attack", 1); 
    }

    public void PerformSpecialAttack(PlayerCombats playerCombats)
    {
        playerCombats.PlaySpecialAttackSound();
        playerCombats.GetComponent<Animator>().SetInteger("Attack", 2);
    }

    public float GetAttackCooldown() => _cooldown;
    public float GetSpecialAttackCooldown() => _cooldown * 1.5f;
}