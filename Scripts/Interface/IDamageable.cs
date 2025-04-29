using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);
    void Heal(int healAmount);
    void Die();
    bool IsDead();
    int GetCurrentHealth();
    int GetMaxHealth();
}
