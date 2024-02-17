using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public void Damage(float damage);
    public void DamageWithKnockback(float damage, Vector2 _direction, float hitStrength);
    public void Die();
}
