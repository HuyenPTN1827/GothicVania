using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class FlyingRetreatState : EnemyRetreatState {
    protected Transform _anchor;
    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);

        _anchor = enemy.Checks.Where(e => e.name.Contains("anchor")).First().transform;
    }
    protected override Vector3 GetSafePosition() {
        if (Vector2.Distance(enemy.transform.position, playerTransform.position) < SafeDistance) {
            return (enemy.transform.position - playerTransform.position).normalized * SafeDistance + enemy.transform.position;
        }
        else if (Vector2.Distance(enemy.transform.position, playerTransform.position) > SafeDistance * CatchUpDistanceMultiplier) {
            return (playerTransform.position - enemy.transform.position).normalized * SafeDistance * CatchUpDistanceMultiplier + enemy.transform.position;
        }
        else {
            if (Vector2.Angle(enemy.transform.position - _anchor.position, enemy.transform.position - playerTransform.position) > 95f) {
                return _anchor.position;
            }
            return enemy.transform.position;
        }
    }
}
