using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable {
    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _knockbackTime = 0.5f;
    

    // Start is called before the first frame update
    void Start() {
        _currentHealth = _maxHealth;
        RB = GetComponent<Rigidbody2D>();
    }
    public void Damage(float damage) {
        _currentHealth -= damage;
        if (_currentHealth < 0) {
            Die();
        }
    }

    public void Die() {
        Destroy(gameObject);
    }

    public void DamageWithKnockback(float damage, Vector2 position, float hitStrength) {
        Damage(damage);


        //StartCoroutine(Knockback(position , hitStrength));
    }

    //private IEnumerator Knockback(Vector2 position, float hitStrength) {
    //    bool _leftHit;
    //    if (position.x < RB.position.x) _leftHit = true;
    //    else _leftHit = false;


    //}
}
