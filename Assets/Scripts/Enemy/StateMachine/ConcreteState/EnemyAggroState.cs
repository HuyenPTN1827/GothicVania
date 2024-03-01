using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class EnemyAggroState : EnemyState {
    [Space(20)]
    [Header("Aggro Values")]
    public float AwareTime;
    public float ChaseSpeedMultiplier;
    public float ChaseVisionMultiplier;
    public float BoundaryRadius;

    protected bool IsOutOfBound;

    private float _retentionTime;
    private GameObject LastPlayerPosition;
    private Transform _anchor;

    public override void AnimationTriggerEvents(Enemy.AnimationTriggerType type) {
        base.AnimationTriggerEvents(type);
    }

    public override void EnemyKilled() {
        if(LastPlayerPosition != null) Destroy(LastPlayerPosition);
    }

    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);

        IsOutOfBound = false;

        _anchor = enemy.Checks.Where(e => e.name.Contains("anchor")).FirstOrDefault()?.transform;
    }

    public override void EnterState() {
        base.EnterState();

        LastPlayerPosition = new GameObject();
        LastPlayerPosition.name = "Last Player Position";
        LastPlayerPosition.transform.position = playerTransform.position;

        _retentionTime = AwareTime;
        enemy.Path.maxSpeed *= ChaseSpeedMultiplier;
        enemy.VisionRange *= ChaseVisionMultiplier;

        enemy.DestinationSetter.target = LastPlayerPosition.transform;
    }

    public override void ExitState() {
        base.ExitState();
    }

    public override void TurnCheck() {
        var xToPlayer = playerTransform.position.x - enemy.transform.position.x;
        if ((xToPlayer < 0 && enemy.IsFacingRight) || (xToPlayer > 0 && !enemy.IsFacingRight)) enemy.Turn();    
    }


    public override void FrameUpdate() {
        base.FrameUpdate();
        _retentionTime -= Time.deltaTime;

        if (_anchor != null) IsOutOfBound = CheckOutOfBound();

        if (enemy.DetectedPlayer) {
            _retentionTime = AwareTime;
            LastPlayerPosition.transform.position = UpdateDestination();
        }

        _retentionTime -= Time.deltaTime;
        if (enemy.DetectedPlayer) _retentionTime = AwareTime;
        if (_retentionTime <= 0 || IsOutOfBound) enemy.StateMachine.ChangeState(enemy.IdleStateInstance);
        if (AvailableAttacks().Count == 0) enemy.StateMachine.ChangeState(enemy.RetreatStateInstance);
        if (enemy.CanAttack) if (TryChooseAttack(out var attack)) enemy.StateMachine.ChangeState(attack);
    }

    protected virtual Vector2 UpdateDestination() => new Vector2(playerTransform.position.x, playerTransform.position.y);

    public bool IsPositionOutOfBound(Vector2 position) {
        if (_anchor == null) return false;
        return Vector2.Distance(position, _anchor?.transform.position ?? enemy.transform.position) > BoundaryRadius;
    }

    public virtual List<EnemyAttackState> AvailableAttacks() {
        List<EnemyAttackState> attacks = new List<EnemyAttackState>();
        foreach (var a in enemy.AttackStateInstances) {
            if (enemy.OnCooldownAttacks.ContainsKey(a) || !a.Prerequisite()) continue;
            attacks.Add(a);
        }
        return attacks;
    }

    public virtual List<EnemyAttackState> AvailableAttacksInRange() {
        List<EnemyAttackState> attacks = new List<EnemyAttackState>();
        foreach (var attack in AvailableAttacks()) {
            if (attack.IsInRangeForAttack()) attacks.Add(attack);
        }
        return attacks;
    }

    public virtual bool TryChooseAttack(out EnemyAttackState attack) {
        attack = null;

        var attacks = AvailableAttacksInRange();

        attacks = attacks.OrderByDescending(e => e.Damage).ThenBy(e => e.AttackCooldown).ThenByDescending(e => e.KnockbackStrength).ToList();

        foreach (var att in attacks) if (att.Prerequisite()) attack = att;

        if (attack == null) return false;
        return true;
    }

    public bool CheckOutOfBound() => Vector2.Distance(enemy.transform.position, _anchor?.transform.position??enemy.transform.position) > BoundaryRadius;

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }

    public override void OnDrawGizmos() {
        //Vision
        var t = enemy.VisionRange / Vector2.Distance(playerTransform.position, enemy.transform.position);
        var vision = new Vector2(((1 - t) * enemy.transform.position.x + t * playerTransform.position.x), ((1 - t) * enemy.transform.position.y + t * playerTransform.position.y));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(vision, enemy.VisionRadius);
        Gizmos.DrawLine(enemy.transform.position, vision);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemy.transform.position, enemy.AwarenessRadius);

        Gizmos.DrawSphere(LastPlayerPosition.transform.position, 0.1f);

        if (_anchor != null)Gizmos.DrawWireSphere(_anchor.position, BoundaryRadius);

        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(enemy.transform.position, enemy.RetreatStateInstance.SafeDistance);

    }

    public override void ResetValues() {
        base.ResetValues();

        enemy.DestinationSetter.target = null;
        enemy.Path.maxSpeed = enemy.Speed;
        enemy.VisionRange /= ChaseVisionMultiplier;

        Destroy(LastPlayerPosition);
    }

    protected override RaycastHit2D[] VisionHit(float visionRadius, float visionRange) => Physics2D.CircleCastAll(enemy.transform.position, visionRadius, (playerTransform.position - enemy.transform.position).normalized, visionRange, enemy._playerLayer);
}
