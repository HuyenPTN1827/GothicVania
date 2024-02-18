using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class BurrowerIdleState : EnemyIdleState {
    [Space(20)]
    [Header("Burrow Values")]
    [SerializeField] public float BurrowSpeedModifier;
    [SerializeField] public float BurrowVisionModifier;
    [SerializeField] public float BurrowAwarenessModifier;

    GameObject _burrowSpot;
    bool _isBetweenAnims = false;
    bool _isSleeping = false;

    float _speed;
    float _vision;
    float _aware;

    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);

        _burrowSpot = new GameObject();
        _burrowSpot.transform.parent = enemy.transform.parent;
        _burrowSpot.transform.position = enemy.transform.position;
    }

    public override void EnterState() {
        base.EnterState();
        _speed = enemy.Speed;
        _vision = enemy.AwarenessRadius;
        _aware = enemy.VisionRange;

        enemy.StartCoroutine(GoToSleep());

    }

    protected virtual IEnumerator GoToSleep() {

        enemy.DestinationSetter.target = _burrowSpot.transform;
        while (!enemy.Path.reachedDestination) {
            if (enemy.ApplyGravity) {
                var enemyPos = enemy.transform.position;
                var burrowPos = _burrowSpot.transform.position;
                if (Math.Abs(burrowPos.x - enemyPos.x) < 0.01f && burrowPos.y - enemyPos.y < 0.5f) {
                    _burrowSpot.transform.position = new Vector3(_burrowSpot.transform.position.x, enemy.transform.position.y, _burrowSpot.transform.position.z);
                }
            }
            yield return null;
        }

        enemy.DestinationSetter.target = null;

        enemy.Speed *= BurrowSpeedModifier;
        enemy.AwarenessRadius *= BurrowAwarenessModifier;
        enemy.VisionRange *= BurrowVisionModifier;

        enemy.Anim.SetTrigger("Unrise");

        _isBetweenAnims = true;
        while (!IsClipNameContains("unrise")) yield return null;

        var unrise = enemy.Anim.GetCurrentAnimatorClipInfo(0).FirstOrDefault(e => e.clip.name.Contains("unrise"));
        yield return new WaitForSeconds(unrise.clip.length * enemy.Anim.speed);
        _isBetweenAnims = false;
        _isSleeping = true;
    }
    bool IsClipNameContains(string name) {
        var prior = enemy.Anim.GetCurrentAnimatorClipInfo(0);
        foreach (var anim in prior) {
            if (anim.clip.name.Contains(name)) return true;
        }
        return false;
    }

    public override void FrameUpdate() {
        base.FrameUpdate();

        //Debug.Log(_isBetweenAnims + " " + enemy.AggroStateInstance.CheckOutOfBound());


        if (enemy.DetectedPlayer && !_isBetweenAnims && !enemy.AggroStateInstance.CheckOutOfBound()) enemy.StartCoroutine(Rise());
    }

    protected virtual IEnumerator Rise() {
        if (!_isSleeping) yield break;
        _isSleeping = false;
        enemy.Anim.SetTrigger("Rise");
        Debug.Log("Hi");
        _isBetweenAnims = true;
        while (!IsClipNameContains("rise")) yield return null;

        var rise = enemy.Anim.GetCurrentAnimatorClipInfo(0).FirstOrDefault(e => e.clip.name.Contains("rise"));
        yield return new WaitForSeconds(rise.clip.length * enemy.Anim.speed);
        _isBetweenAnims = false;

        enemy.StateMachine.ChangeState(enemy.AggroStateInstance);
    }

    public override void ResetValues() {
        base.ResetValues();

        enemy.Speed = _speed;
        enemy.AwarenessRadius = _aware;
        enemy.VisionRange = _vision;
    }
}
