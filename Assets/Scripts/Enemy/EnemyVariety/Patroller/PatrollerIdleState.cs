using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollerIdleState : EnemyIdleState {
    [SerializeField] protected Transform pointA;
    [SerializeField] protected Transform pointB;

    [Header("Patrol parameters")]
    [Range(-1, 1)]
    [SerializeField] protected int DefaultDestination = 0;
    [SerializeField] protected float Speed = 1f;

    Transform _destination;

    public PatrollerIdleState(Enemy _enemy, EnemyStateMachine _enemyStateMachine) : base(_enemy, _enemyStateMachine) {
    }

    public override void AnimationTriggerEvents(Enemy.AnimationTriggerType type) {
        base.AnimationTriggerEvents(type);
    }

    public override void EnterState() {
        base.EnterState();

        switch (DefaultDestination) {
            case 1:
                _destination = pointA;
                break;
            case 2:
                _destination = pointB;
                break;
            default:
                if (Vector2.Distance(enemy.transform.position, pointA.position) <= Vector2.Distance(enemy.transform.position, pointB.position)) _destination = pointA;
                else _destination = pointB;
                break;

        }
    }

    public override void ExitState() {
        base.ExitState();
    }

    public override void FrameUpdate() {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();

        //enemy.RB.MovePosition()
    }
}
