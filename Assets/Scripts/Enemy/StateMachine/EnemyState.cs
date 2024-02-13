using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState {
    protected Enemy enemy;
    protected EnemyStateMachine enemyStateMachine;

    public EnemyState(Enemy _enemy, EnemyStateMachine _enemyStateMachine) {
        enemy = _enemy;
        enemyStateMachine = _enemyStateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void AnimationTriggerEvents(Enemy.AnimationTriggerType type) { }
}
