using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable {

    #region Healths
    public float MaxHealth { get => _maxHealth; set { _maxHealth = value; } }
    public float CurrentHealth { get => _currentHealth; set { _currentHealth = value; } }
    [SerializeField] float _maxHealth;
    [SerializeField] float _currentHealth;

    Enemy enemy;

    #endregion

    [SerializeField] private float _knockbackTime = 5f;
    [SerializeField] private float _knockbackCount = 0;
    [SerializeField] private bool _ignoreKnockback;

    void Awake() {
        CurrentHealth = MaxHealth;
        enemy = GetComponent<Enemy>();
    }
    
    public void Damage(float damage) {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0) {
            Die();
        }
    }

    public void Die() {
        enemy.RB.velocity = Vector2.zero;
        enemy.RB.gravityScale = 0f;
        enemy.Collider.enabled = false;
        enemy.Anim.SetTrigger("Die");
    }

    public void SelfKill() => Destroy(transform.gameObject);

    public void OnDestroy() {
        
    }

    private void Update() {
    }

    public void DamageWithKnockback(float damage, Vector2 position, float hitStrength) {
        if (_ignoreKnockback) StartCoroutine(Recovery(position, hitStrength));

        Damage(damage);
        //StartCoroutine(Knockback(position , hitStrength));
    }

    IEnumerator Recovery(Vector2 position, float hitStrength) {
        var target = enemy.DestinationSetter.target;
        enemy.DestinationSetter.target = null;
        enemy.Path.canMove = false;
        enemy.DestinationSetter.enabled = false;
        enemy.RB.velocity = Vector2.zero;
        enemy.RB.velocity = new Vector2(0.1f, 0.9f) * (position.x - enemy.transform.position.x < 0 ? 1 : -1) * hitStrength;
        yield return new WaitForSeconds(_knockbackTime);
        enemy.Path.canMove = true;
        enemy.DestinationSetter.enabled = true;
    }

    //private IEnumerator Knockback(Vector2 position, float hitStrength) {
    //    bool _leftHit;
    //    if (position.x < RB.position.x) _leftHit = true;
    //    else _leftHit = false;


    //}
}
