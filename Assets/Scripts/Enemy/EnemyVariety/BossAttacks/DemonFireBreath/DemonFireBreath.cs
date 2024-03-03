using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CreateAssetMenu]
public class DemonFireBreath : EnemyAttackState {
    [SerializeField] protected float TickRate;
    [SerializeField] protected float BreathingTime;
    [SerializeField] protected float _breathRadius;
    [SerializeField] protected float _breathingSpeedModifier;

    static int count = 0;
    protected float _breathTimeRemaining = 0;
    protected float _tickCountdown = 0;

    protected Transform _breathCenter;
    protected float _speed;

    public override void Initialize(GameObject gameObject, Enemy enemy) {
        count = 0;
        base.Initialize(gameObject, enemy);
        _breathCenter = enemy.Checks.Find(e => e.name.Equals("breathCenter")).transform;
    }

    public override void EnterState() {
        base.EnterState();
        _breathTimeRemaining = 99f;

        _speed = enemy.Speed;
        enemy.Speed *= _breathingSpeedModifier;
    }

    public override void ExitState() {
        base.ExitState();
        enemy.Speed = _speed;
    }

    public override void ExecuteHit() {
        foreach (var target in targets) {
            IDamageable damageable = target.GetComponent<IDamageable>();
            Vector2 direction = (pos - target.transform.position).normalized;
            direction = new Vector2(KnockbackVertical.x * (direction.x < 0 ? 1 : -1), KnockbackVertical.y);
            damageable.Damage(Damage);
        }
    }

    public override void FrameUpdate() {
        base.FrameUpdate();

        _tickCountdown -= Time.deltaTime;
        _breathTimeRemaining -= Time.deltaTime;

        if (_tickCountdown < 0f ) {
            _tickCountdown = TickRate;
            ExecuteHit();
            targetsTotal.Clear();
        }
        if (_breathTimeRemaining < 0) {
            HitEnd();
        }
    }

    public override void HitStart() {
        _breathTimeRemaining = BreathingTime;
        enemy.Anim.SetBool("IsBreathing", true);
    }

    public override void HitEnd() {
        targetsTotal.Clear();

        enemy.StartCoroutine(StartAttackCooldown());
        _canChangeState = true;
        enemy.Anim.SetBool("IsBreathing", false);
    }

    public override bool IsInRangeForAttack() => Physics2D.CircleCastAll
        (_breathCenter.position, _breathRadius, -enemy.transform.right, 0f, enemy._playerLayer).Length
        > 0;

    protected override RaycastHit2D[] GetHits() => Physics2D.CircleCastAll(_breathCenter.position, _breathRadius, Vector2.zero, 0f, enemy._playerLayer);

    public override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_breathCenter.position, _breathRadius);
    }
}
