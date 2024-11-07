using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);

    void Heal(float healAmount);

    void Die();

    Transform GetTransform();
}