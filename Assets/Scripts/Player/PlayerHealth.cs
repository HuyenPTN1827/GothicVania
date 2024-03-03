using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable {
    [SerializeField] Rigidbody2D RB;
    [SerializeField] Animator Anim;
    [SerializeField] PlayerRespawn Respawn;
    private PlayerMovement movement;

    #region Healths
    [SerializeField] float _maxHealth;
    [SerializeField] float _currentHealth;


    public float MaxHealth { get => _maxHealth; set { _maxHealth = value; } }
    public float CurrentHealth { get => _currentHealth; set { _currentHealth = value; } }
    #endregion

    [Space(20)]
    [Header("Knockback Values")]
    public float knocbackDamage;
    public Vector2 knockbackVertical;
    public float knockbackStrength;
    [SerializeField] float _recoveryTime;
    [SerializeField] float _invicibleTime;
    float _recoveringTime = 0;
    float _iframe = 0;


    void Start() {
        CurrentHealth = MaxHealth;
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        Respawn = GetComponent<PlayerRespawn>();
    }

    void Update() {

        _recoveringTime -= Time.deltaTime;
        _iframe -= Time.deltaTime;

        if (_recoveringTime < 0) EndRecovery();
    }

    public void Damage(float damage) {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0) Die();
    }

    public void Heal(float heal) {
        CurrentHealth += heal;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
    }

    public void DamageWithKnockback(float damage, Vector2 _direction, float hitStrength) {
        Damage(damage);

        Anim.SetBool("Hit", true);
        StartRecovery();
        StartInvicible();
        RB.velocity = Vector2.one * 0.1f;
        RB.velocity = _direction * hitStrength;
    }

    private void StartInvicible() {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Monster"), true);
        _iframe = _invicibleTime + _recoveryTime;
        StartCoroutine(EndInvicible());
    }

    IEnumerator EndInvicible() {
        yield return new WaitForSeconds(_invicibleTime);

        var monsterLayer = LayerMask.NameToLayer("Monster");
        var collider = GetComponent<BoxCollider2D>();
        if (Physics2D.OverlapBox(transform.position, collider.size, 0f, monsterLayer)) {
            var hits = Physics2D.BoxCastAll(transform.position + new Vector3(collider.offset.x * transform.localScale.x, collider.offset.y * transform.localScale.y, 0f), (collider.size * transform.localScale), 0f, Vector2.zero, 0f, monsterLayer);
            foreach (var hit in hits) {
                if (hit != null) {
                    Vector2 direction = (hit.transform.position - transform.position).normalized;
                    direction = new Vector2(knockbackVertical.x * (direction.x < 0 ? 1 : -1), knockbackVertical.y);
                    DamageWithKnockback(knocbackDamage, direction, knockbackStrength);
                    break;
                }
            }
        }
        else Physics2D.IgnoreLayerCollision(gameObject.layer, monsterLayer, false);
    }

    private void OnDrawGizmos() {
        //var collider = GetComponent<BoxCollider2D>();
        //Gizmos.DrawWireCube(transform.position + new Vector3(collider.offset.x * transform.localScale.x, collider.offset.y * transform.localScale.y, 0f), (collider.size * transform.localScale) );
    }

    private void StartRecovery() {
        if (!movement.LockInput) movement.LockInput = true;
        _recoveringTime = _recoveryTime;
    }

    private void EndRecovery() {
        var movement = GetComponent<PlayerMovement>();
        if (movement.LockInput) movement.LockInput = false;
        Anim.SetBool("Hit", false);
    }

    public void Die() {
        Debug.Log("Player Died");
        CurrentHealth = MaxHealth;

        Respawn?.Respawn();
    }

    public void DamageWithRespawn(float damage = 0f) {
        Damage(damage);

        //RB.velocity = Vector3.zero;
        StartInvicible();
        if (CurrentHealth > 0f) Respawn.Respawn();
    }

}
