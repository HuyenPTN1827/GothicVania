using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyAttackState : EnemyState {
    [Space(20)]
    [Header("Attack Values")]
    [SerializeField] public Transform AttackPosition;
    [SerializeField] public float AttackRange;
    [SerializeField] public float AttackCooldown;
    [SerializeField] public string AttackAnimTrigger;
    [SerializeField] public float Damage;
    [SerializeField] protected bool WaitUntilHitEnd;
    [SerializeField] protected Vector2 KnockbackVertical;
    [SerializeField] public float KnockbackStrength;

    protected List<GameObject> targets;
    protected List<GameObject> targetsTotal;
    protected Vector3 pos;
    protected bool _canChangeState;
    bool _isOnCooldown;
    public override void AnimationTriggerEvents(Enemy.AnimationTriggerType type) {
        base.AnimationTriggerEvents(type);
    }

    public virtual bool Prerequisite() => true;

    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);

        _isOnCooldown = false;

        AttackPosition = enemy.Checks.Find(e => e.name.Equals("attackOrigin")).transform;
    }

    public override void EnterState() {
        base.EnterState();

        targets = new List<GameObject>();
        targetsTotal = new List<GameObject>();

        _canChangeState = true;
    }

    public override void ExitState() {
        base.ExitState();
    }

    public override void FrameUpdate() {

        base.FrameUpdate();

        if (!WaitUntilHitEnd) ExecuteHit();

        if (IsInRangeForAttack() && !_isOnCooldown) Attack();
        else {
            if (_canChangeState) enemy.StateMachine.ChangeState(enemy.AggroStateInstance);
        }
    }

    protected virtual void Attack() {
        if (_isOnCooldown) return;

        pos = enemy.transform.position;

        enemy.Anim.SetTrigger(AttackAnimTrigger);
        enemy.OnCooldownAttacks.Add(this, 999f);
        _canChangeState = false;

        _isOnCooldown = true;
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }

    public override void TurnCheck() {
        var xToPlayer = playerTransform.position.x - enemy.transform.position.x;
        if ((xToPlayer < 0 && enemy.IsFacingRight) || (xToPlayer > 0 && !enemy.IsFacingRight)) enemy.Turn();
    }

    public virtual void HitStart() { enemy.StartCoroutine(StartHitting()); }

    protected virtual IEnumerator StartHitting() {
        while (!_canChangeState) {
            var hits = GetHits();
            foreach (var hit in hits) {
                var go = hit.collider.gameObject;
                if (Physics2D.GetIgnoreLayerCollision(enemy.gameObject.layer, go.layer)) continue;
                if (targetsTotal.Contains(go) || go.GetComponent<IDamageable>() == null) continue;
                targets.Add(go);
                targetsTotal.Add(go);
            }
            yield return null;
        }
    }

    protected virtual RaycastHit2D[] GetHits() => Physics2D.CircleCastAll(AttackPosition.transform.position, AttackRange, Vector2.zero, 0f, enemy._playerLayer);

    public virtual void HitEnd() {
        ExecuteHit();
        targetsTotal.Clear();

        enemy.StartCoroutine(StartAttackCooldown());
        _canChangeState = true;
    }

    protected IEnumerator StartAttackCooldown() {
        enemy.OnCooldownAttacks[this] = AttackCooldown;
        yield return new WaitForSeconds(AttackCooldown);
        _isOnCooldown = false;
    }

    public virtual void ExecuteHit() {
        foreach (var target in targets) {
            IDamageable damageable = target.GetComponent<IDamageable>();
            Vector2 direction = (pos - target.transform.position).normalized;
            direction = new Vector2(KnockbackVertical.x * (direction.x < 0 ? 1 : -1), KnockbackVertical.y);
            damageable.DamageWithKnockback(Damage, direction, KnockbackStrength);
        }
    }

    public virtual bool IsInRangeForAttack() => Physics2D.CircleCastAll
        (AttackPosition.position, AttackRange, -enemy.transform.right, 0f, enemy._playerLayer).Length
        > 0;

    public override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(AttackPosition.transform.position, AttackRange);
    }
}
