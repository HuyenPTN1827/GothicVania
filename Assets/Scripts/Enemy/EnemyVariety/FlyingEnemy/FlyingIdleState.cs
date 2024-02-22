using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class FlyingIdleState : EnemyIdleState {
    Transform _anchor;
    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);
    }

    public override void EnterState() {
        base.EnterState();

        _anchor = enemy.Checks.Where(e => e.name.Contains("anchor")).FirstOrDefault()?.transform;

        if (_anchor == null) Debug.LogError("Anchor is not added");

        enemy.DestinationSetter.target = _anchor;
    }

    public override void FrameUpdate() {
        base.FrameUpdate();
        if (enemy.DetectedPlayer && !enemy.AggroStateInstance.CheckOutOfBound()) enemy.StateMachine.ChangeState(enemy.AggroStateInstance);
    }


}
