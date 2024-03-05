using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CreateAssetMenu]
public class DemonFireBreath : EnemyAttackState {
    [Space(20)]
    [Header("Breath Values")]
    [SerializeField] protected float TickRate;
    [SerializeField] protected float BreathingTime;
    [SerializeField] protected float _breathRadius;
    [SerializeField] protected float _breathingSpeedModifier;

    [Space(20)]
    [Header("Spawn item after breath")]
    [SerializeField] List<Item> _items;

    static int count = 0;
    protected float _breathTimeRemaining = 0;
    protected float _tickCountdown = 0;
    protected bool _isSpawning = false;

    protected Transform _breathCenter;
    protected float _speed;

    public override void Initialize(GameObject gameObject, Enemy enemy) {
        count = 0;
        base.Initialize(gameObject, enemy);
        _breathCenter = enemy.Checks.Find(e => e.name.Equals("breathCenter")).transform;
    }

    public override void EnterState() {
        base.EnterState();
        _breathTimeRemaining = 99f;

        _speed = enemy.Speed;
        enemy.Speed *= _breathingSpeedModifier;
    }

    public override void ExitState() {
        base.ExitState();
        enemy.Speed = _speed;
    }

    public override void ExecuteHit() {
        foreach (var target in targets) {
            IDamageable damageable = target.GetComponent<IDamageable>();
            Vector2 direction = (pos - target.transform.position).normalized;
            direction = new Vector2(KnockbackVertical.x * (direction.x < 0 ? 1 : -1), KnockbackVertical.y);
            damageable.Damage(Damage);
        }
    }

    public override void FrameUpdate() {
        base.FrameUpdate();

        _tickCountdown -= Time.deltaTime;
        _breathTimeRemaining -= Time.deltaTime;

        if (_tickCountdown < 0f) {
            _tickCountdown = TickRate;
            Debug.Log("tick");
            ExecuteHit();
            targetsTotal.Clear();
        }
        if (_breathTimeRemaining < 0) {
            //Debug.Log("Spawn item");
            HitEnd();
        }
    }

    public override void HitStart() {
        base.HitStart();
        _breathTimeRemaining = BreathingTime;
        enemy.Anim.SetBool("IsBreathing", true);
    }

    public override void HitEnd() {
        targetsTotal.Clear();

        enemy.StartCoroutine(StartAttackCooldown());
        _canChangeState = true;
        enemy.Anim.SetBool("IsBreathing", false);

        enemy.StartCoroutine(SpawnItem());
    }

    private IEnumerator SpawnItem() {
        if (_isSpawning) yield break;
        _isSpawning = true;


        if (_items != null) {
            Debug.Log("Spawn item");
            var id = new System.Random().Next(0, _items.Count);
            //Debug.Log("Count = " + (_items.Count) + " Id = " + id);
            var item = Instantiate(_items[id]);
            item.transform.position = _breathCenter.position;
        }

        _isSpawning = false;
    }

    public override bool IsInRangeForAttack() => Physics2D.CircleCastAll
        (_breathCenter.position, _breathRadius, -enemy.transform.right, 0f, enemy._playerLayer).Length
        > 0;

    protected override RaycastHit2D[] GetHits() => Physics2D.CircleCastAll(_breathCenter.position, _breathRadius, Vector2.zero, 0f, enemy._playerLayer);

    public override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_breathCenter.position, _breathRadius);
    }
}
