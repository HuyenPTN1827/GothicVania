using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : ScriptableObject {
    protected internal Enemy enemy;
    protected internal Transform transform;
    protected internal GameObject gameObject;

    protected internal Transform playerTransform;

    public virtual void Initialize (GameObject gameObject, Enemy enemy) {
        this.gameObject = gameObject;
        this.enemy = enemy;
        transform = gameObject.transform;


        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void DetectingPlayer() {
        var hits = VisionHit(enemy.VisionRadius, enemy.VisionRange);
        var aware = VisionHit(enemy.AwarenessRadius, 0f);
        if (aware.Length != 0 || hits.Length != 0) enemy.DetectedPlayer = true;
        else enemy.DetectedPlayer = false;
    }

    protected virtual RaycastHit2D[] VisionHit(float visionRadius, float visionRange) => Physics2D.CircleCastAll(enemy.transform.position, visionRadius, -enemy.transform.right * (enemy.IsFacingRight ? -1 : 1), visionRange, enemy._playerLayer);

    public virtual void EnterState() { /*Debug.Log(this.name);*/ }
    public virtual void ExitState() { ResetValues(); }
    public virtual void FrameUpdate() {
        DetectingPlayer();
    }
    public virtual void PhysicsUpdate() { TurnCheck(); }

    public virtual void TurnCheck() {
        var x = enemy.PrevPos.x - enemy.transform.position.x;
        if ((x > 0 && enemy.IsFacingRight) || (x < 0 && !enemy.IsFacingRight)) enemy.Turn();
    }
    public virtual void AnimationTriggerEvents(Enemy.AnimationTriggerType type) { }
    public virtual void ResetValues() { }
    public virtual void EnemyKilled() { }
    public virtual void OnDrawGizmos() {
        //Vision
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(enemy.transform.position, enemy.VisionRange * 2);
        Gizmos.DrawWireSphere(enemy.transform.position - enemy.transform.right * (enemy.IsFacingRight ? -1 : 1) * enemy.VisionRange, enemy.VisionRadius);
        Gizmos.DrawLine(enemy.transform.position, enemy.transform.position + enemy.transform.right * (enemy.IsFacingRight ? 1 : -1) * enemy.VisionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemy.transform.position, enemy.AwarenessRadius);
        Gizmos.color = Color.white;
    }
}
