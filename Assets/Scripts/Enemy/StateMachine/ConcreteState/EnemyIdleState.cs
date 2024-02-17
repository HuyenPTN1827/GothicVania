using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyIdleState : EnemyState {
    public override void AnimationTriggerEvents(Enemy.AnimationTriggerType type) {
        base.AnimationTriggerEvents(type);
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void ExitState() {
        base.ExitState();
    }

    public override void FrameUpdate() {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
