using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    public void Damage(float damage);
    public void DamageWithKnockback(float damage, Vector2 _position, float hitStrength);
    public void Die();
}
