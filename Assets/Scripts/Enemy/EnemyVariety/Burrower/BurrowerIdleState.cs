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

    #region Check Values
    GameObject _burrowSpot;
    bool _isBetweenAnims = false;
    bool _isSleeping = false;
    #endregion

    #region Stored values
    float _speed;
    float _vision;
    float _aware;
    Vector2 _colliderOffset;
    Vector2 _colliderSize;
    #endregion

    public override void EnemyKilled() {
        base.EnemyKilled();
        Destroy(_burrowSpot);
    }

    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);


        _burrowSpot = new GameObject();
        _burrowSpot.name = "Burrow " + enemy.name;
        _burrowSpot.transform.parent = enemy.transform.parent;
        _burrowSpot.transform.position = enemy.transform.position;
    }

    public override void EnterState() {
        base.EnterState();
        _speed = enemy.Speed;
        _vision = enemy.VisionRange;
        _colliderOffset = enemy.GetComponent<BoxCollider2D>().offset;
        _colliderSize = enemy.GetComponent<BoxCollider2D>().size;
        _aware = enemy.AwarenessRadius;

        Debug.Log(_aware);
        enemy.StartCoroutine(GoToSleep());

    }

    protected virtual IEnumerator GoToSleep() {

        enemy.DestinationSetter.target = _burrowSpot.transform;
        while (!enemy.Path.reachedDestination) {
            if (enemy.DetectedPlayer && !enemy.AggroStateInstance.CheckOutOfBound() && !enemy.AggroStateInstance.IsPositionOutOfBound(playerTransform.position)) {
                enemy.StateMachine.ChangeState(enemy.AggroStateInstance);
                yield break;
            }
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


        enemy.Anim.SetTrigger("Unrise");

        _isBetweenAnims = true;
        while (!IsClipNameContains("unrise")) yield return null;

        var unrise = enemy.Anim.GetCurrentAnimatorClipInfo(0).FirstOrDefault(e => e.clip.name.Contains("unrise"));
        yield return new WaitForSeconds(unrise.clip.length * enemy.Anim.speed);
        _isBetweenAnims = false;
        _isSleeping = true;

        enemy.Collider.offset += Vector2.down * (_colliderSize.y / 2);
        ((BoxCollider2D)enemy.Collider).size = new Vector2(_colliderSize.x, _colliderSize.y / 6);
        enemy.Speed *= BurrowSpeedModifier;
        enemy.AwarenessRadius *= BurrowAwarenessModifier;
        enemy.VisionRange *= BurrowVisionModifier;
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

        //Debug.Log(enemy.DetectedPlayer + " " + _isBetweenAnims + " " + enemy.AggroStateInstance.CheckOutOfBound());


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

        enemy.Collider.offset = _colliderOffset;
        ((BoxCollider2D) enemy.Collider).size = _colliderSize;
        enemy.Speed = _speed;
        enemy.AwarenessRadius = _aware;
        enemy.VisionRange = _vision;

    }
}
