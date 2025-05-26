public interface IGun //IWeapon занято ;(
{
    public float Damage { get; }
    public float Range { get; }
    public float SpecialDamage { get; }

    public float SpecialRange { get; }


    void PerformAttack(PlayerCombats playerCombats);
    void PerformSpecialAttack(PlayerCombats playerCombats);
    float GetAttackCooldown();
    float GetSpecialAttackCooldown();
}