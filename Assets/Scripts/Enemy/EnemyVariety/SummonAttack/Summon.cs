using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Summon : MonoBehaviour, ISummon {

    [SerializeField] public LayerMask _hitLayer;
    [Range(0f, 1f)][SerializeField] public float BoundXModifier;
    [Range(0f, 1f)][SerializeField] public float BoundYModifier;
    [SerializeField] public bool CreateColliderOnHitStart;
    [SerializeField] public bool DestroyOnCollision;

    [SerializeField] public float Damage;
    [SerializeField] public float DamageTickRate;
    [SerializeField] public Vector2 Knockback;
    [SerializeField] public float KnockbackStrength;

    protected Enemy enemy;
    protected EnemyAttackState attackState;

    SpriteRenderer SR;
    bool _performingHit;
    List<IDamageable> damageables;
    float _tickCooldown;


    private void Start() {
        SR = GetComponent<SpriteRenderer>();
        _performingHit = false;
        damageables = new List<IDamageable>();
    }

    private void Update() {
        _tickCooldown -= Time.deltaTime;

        if (_tickCooldown < 0 ) {
            _tickCooldown = DamageTickRate;
            ExecuteHit();
        }
    }

    public virtual void Initialize(EnemySummonState state, Vector2 spawnLocation) {
        transform.position = spawnLocation;

        enemy = state.enemy;
        attackState = state;
    }

    public virtual void PerformHit() {
        if (CreateColliderOnHitStart) {
            AddCollider();
        }
        else StartCoroutine(StartRecordHit());
    }

    public virtual void AddCollider() {
        var bc = gameObject.GetComponent<BoxCollider2D>();
        bc.size = new Vector2((SR.size.x * BoundXModifier)/ 2, (SR.size.y * BoundYModifier)/2);
        bc.enabled = true;
    }

    protected IEnumerator StartRecordHit() {
        while (_performingHit) {
            var hits = Physics2D.BoxCastAll(new Vector2(transform.position.x, transform.position.y + +SR.bounds.size.y / 2), new Vector2(SR.bounds.size.x * BoundXModifier, SR.bounds.size.y * BoundYModifier), 0f, Vector2.zero, 0f, _hitLayer);
            foreach (var hit in hits) {
                if (hit.collider.GetComponent<IDamageable>() == null) continue;
                damageables.Add(hit.collider.GetComponent<IDamageable>());
            }
            yield return null;
        }

    }



    private void ExecuteHit() {
        var knockbackVector = new Vector2(Knockback.x * (enemy.transform.position.x > attackState.playerTransform.position.x ? 1 : -1), Knockback.y);
        foreach (var damage in damageables) {
            if (GetComponent<Collider2D>() != null) damage.DamageWithKnockback(Damage, knockbackVector, KnockbackStrength);
        }
        damageables.Clear();
    }

    public virtual void OnAnticipateAnim() { }

    public virtual void OnEnemyDestroy() => DestroySummon();

    public virtual void OnHitEnd() { }

    public virtual void OnHitStart() { }

    public virtual void OnStrikeAnim() { }

    public virtual void DestroySummon() => Destroy(gameObject);

    private void OnCollisionEnter2D(Collision2D collision) {
        var knockbackVector = new Vector2(Knockback.x * (enemy.transform.position.x < attackState.playerTransform.position.x ? 1 : -1), Knockback.y);
        if (collision.collider.GetComponent<IDamageable>() != null) {
            var damage = collision.collider.GetComponent<IDamageable>();
            damage.DamageWithKnockback(Damage, knockbackVector, KnockbackStrength);
        }
        if (DestroyOnCollision) DestroySummon();
    }

    public void OnStrikeAnimEnd() => DestroySummon();
}
