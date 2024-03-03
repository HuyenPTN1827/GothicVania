using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapColliding : MonoBehaviour {
    [SerializeField] float Damage;
    private void OnCollisionEnter2D(Collision2D collision) {
        var damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null) {
            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Player"))) HitPlayer(collision);
            else if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Monster"))) HitMonster(collision);
            else Debug.Log("Trap hit " +  collision.gameObject.name);
        }
    }



    private void HitMonster(Collision2D collision) {
        EnemyHealth damageable = collision.gameObject.GetComponent<IDamageable>() as EnemyHealth;
        damageable.Die();
    }

    private void HitPlayer(Collision2D collision) {
        PlayerHealth damageable = collision.gameObject.GetComponent<IDamageable>() as PlayerHealth;
        damageable.DamageWithRespawn(Damage);
    }
}
