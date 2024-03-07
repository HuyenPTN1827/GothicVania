using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CreateAssetMenu]
public class DemonFireBreath : EnemyAttackState {
    public AudioManager audioManager;
    private bool isBreathing = false;

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
    protected GameObject _spawnItem;
    protected bool IsSpawning = false;

    protected Transform _breathCenter;
    protected float _speed;

    public override void Initialize(GameObject gameObject, Enemy enemy) {
        count = 0;
        base.Initialize(gameObject, enemy);
        _breathCenter = enemy.Checks.Find(e => e.name.Equals("breathCenter")).transform;
        _spawnItem = null;
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
        targets.Clear();
        targetsTotal.Clear();
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();

        _tickCountdown -= Time.deltaTime;
        _breathTimeRemaining -= Time.deltaTime;

        if (_tickCountdown < 0f) {
            _tickCountdown = TickRate;
            //Debug.Log("tick");
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
        enemy.StartCoroutine(BreathFire());
        //audioManager.StartBreathFire(audioManager.flameClip);
    }

    IEnumerator BreathFire()
    {
        if (!isBreathing)
        {
            isBreathing = true;
            audioManager.StartBreathFire(audioManager.flameClip);

            yield return new WaitForSeconds(BreathingTime);

            audioManager.StopBreathFire(audioManager.flameClip);
            isBreathing = false;
        }
    }

    public override void HitEnd() {
        targetsTotal.Clear();
        enemy.StartCoroutine(StartAttackCooldown());
        enemy.StopCoroutine(BreathFire());
        _canChangeState = true;
        enemy.Anim.SetBool("IsBreathing", false);
        isBreathing = false;

        SpawnItem();
    }

    private void SpawnItem() {
        if (IsSpawning) return;
        IsSpawning = true;

        var id = new System.Random().Next(0, _items.Count);
        //Debug.Log("Count = " + (_items.Count) + " Id = " + id
        var item = Instantiate(_items[id]);
        _spawnItem = item.gameObject;
        Debug.Log("Spawn item " +item.name);
        item.transform.position = _breathCenter.position;
        IsSpawning = false;
    }

    public override bool IsInRangeForAttack() => Physics2D.CircleCastAll
        (_breathCenter.position, _breathRadius, -enemy.transform.right, 0f, enemy._playerLayer).Length
        > 0;

    public override void ResetValues() {
        base.ResetValues();

        _spawnItem = null;
    }
    protected override RaycastHit2D[] GetHits() => Physics2D.CircleCastAll(_breathCenter.position, _breathRadius, Vector2.zero, 0f, enemy._playerLayer);

    public override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_breathCenter.position, _breathRadius);
    }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
}
