using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyRetreatState : EnemyState {
    [Space(20)]
    [Header("Retreat Values")]
    [SerializeField] public float SafeDistance;
    [SerializeField] public float CatchUpDistanceMultiplier = 1.2f;
    [SerializeField] public float RetreatSpeedModifier;

    int _cooldownCount;
    float _speed;

    GameObject _safety;

    public override void EnterState() {
        base.EnterState();

        _safety = new GameObject();
        _safety.name = "Safety";
        _safety.transform.position = GetSafePosition();
        _safety.transform.parent = enemy.transform.parent;

        _speed = enemy.Speed;

        _cooldownCount = enemy.OnCooldownAttacks.Count;

        enemy.DestinationSetter.target = _safety.gameObject.transform;
    }

    protected virtual Vector3 GetSafePosition() {
        if (Vector2.Distance(enemy.transform.position, playerTransform.position) < SafeDistance) {
            return (enemy.transform.position - playerTransform.position).normalized * SafeDistance + enemy.transform.position;
        }
        else if (Vector2.Distance(enemy.transform.position, playerTransform.position) > SafeDistance * CatchUpDistanceMultiplier) {
            return (playerTransform.position - enemy.transform.position).normalized * SafeDistance *CatchUpDistanceMultiplier + enemy.transform.position;
        }
        return enemy.transform.position;
        //return Mathf.Abs(enemy.VisionRange - SafeDistance) / 2 * (playerTransform.position -enemy.transform.position).normalized + enemy.transform.position;
    }

    public override void FrameUpdate() {
        base.FrameUpdate();


        if (Vector2.Distance(enemy.transform.position, playerTransform.position) < SafeDistance) {
            var ratio = (SafeDistance - Vector2.Distance(enemy.transform.position, playerTransform.position)) / SafeDistance;
            //Debug.Log(ratio + 1f);
            enemy.Speed = (enemy.Speed * RetreatSpeedModifier) * (ratio + 1f);
        }

            if (enemy.VisionRange <= SafeDistance && enemy.AwarenessRadius <= SafeDistance) Debug.LogWarning("Safe distance must be smaller than Vision Range");
        _safety.transform.position = GetSafePosition();

        if (_cooldownCount != enemy.OnCooldownAttacks.Count) enemy.StateMachine.ChangeState(enemy.AggroStateInstance);
    }

    //public override void TurnCheck() {
    //    var xToPlayer = playerTransform.position.x - enemy.transform.position.x;
    //    if ((xToPlayer < 0 && enemy.IsFacingRight) || (xToPlayer > 0 && !enemy.IsFacingRight)) enemy.Turn();
    //}
    
    protected override RaycastHit2D[] VisionHit(float visionRadius, float visionRange) => Physics2D.CircleCastAll(enemy.transform.position, visionRadius, (playerTransform.position - enemy.transform.position).normalized, visionRange, enemy._playerLayer);

    public override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(enemy.transform.position, SafeDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemy.transform.position, SafeDistance * CatchUpDistanceMultiplier);
        if (_safety != null) Gizmos.DrawSphere(_safety.transform.position, 0.1f);
    }

    public override void ExitState() {
        base.ExitState();

        enemy.Speed = _speed;

        enemy.DestinationSetter.target = null;
        Destroy(_safety);
    }

    public override void EnemyKilled() {
        base.EnemyKilled();

        if (_safety != null) Destroy(_safety);
    }

}
