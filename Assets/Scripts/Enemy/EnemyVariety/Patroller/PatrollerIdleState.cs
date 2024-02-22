using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class PatrollerIdleState : EnemyIdleState {
    [SerializeField] protected List<Transform> points;
    List<Transform> list;

    [Header("Patrol parameters")]
    [SerializeField] protected PatrolType patrolType;
    [SerializeField] protected float RestTimeBetweenPoints = 0.3f;

    float _restingTime = 0;
    Transform _destination;


    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);

        foreach (var point in enemy.Checks.Where(e => e.name.Contains("patrol"))) {
            points.Add(point.transform);
        }
        enemy.DestinationSetter.target = points[0];
    }

    public override void AnimationTriggerEvents(Enemy.AnimationTriggerType type) {
        base.AnimationTriggerEvents(type);
    }

    public override void EnterState() {
        base.EnterState();

        list = points;
        _destination = list[0];
        //Go to nearest point in list
        foreach (Transform point in list) {
            if (Vector2.Distance(enemy.transform.position, point.position) < Vector2.Distance(enemy.transform.position, _destination.position)) _destination = point;
        }
        enemy.DestinationSetter.target = _destination;
    }

    public override void ExitState() {
        base.ExitState();
    }

    public override void FrameUpdate() {
        base.FrameUpdate();

        _restingTime -= Time.deltaTime;
        if (enemy.Path.reachedDestination && enemy.DestinationSetter.target == _destination) {
            enemy.DestinationSetter.target = null;
        }
        if (enemy.DestinationSetter.target == null) enemy.DestinationSetter.target = GetNextPoint();

        if (enemy.DetectedPlayer && !enemy.AggroStateInstance.CheckOutOfBound()) enemy.StateMachine.ChangeState(enemy.AggroStateInstance);
    }

    private Transform GetNextPoint() {
        if (list.Count == 1) return _destination;
        switch (patrolType) {
            case PatrolType.Cycle:
                int c_Index = list.IndexOf(_destination);
                if (c_Index != list.Count - 1) _destination = list[c_Index + 1];
                else _destination = list[0];
                break;
            case PatrolType.Random:
                var ra_dest = _destination;
                while (ra_dest == _destination) {
                    ra_dest = list[Random.Range(0, list.Count)];
                }
                _destination = ra_dest;
                break;
            case PatrolType.Reverse:
            default:
                int r_Index = list.IndexOf(_destination);
                if (r_Index == list.Count - 1) list.Reverse();
                _destination = list[list.IndexOf(_destination) + 1];
                break;
        }
        return _destination;
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }

    public enum PatrolType {
        Cycle,
        Reverse,
        Random
    }
}
