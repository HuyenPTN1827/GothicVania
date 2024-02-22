using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    [SerializeField] public AnimationClip SpawnClip;
    [SerializeField] public AnimationClip FlyingClip;
    [SerializeField] public Animator anim;
    [SerializeField] public float FlySpeed;
    [SerializeField] public bool WaitToShoot;
    [SerializeField] public bool TrackPlayer;
    [SerializeField] public float SteeringSpeed;
    [SerializeField] Vector2 _currentDirection = Vector2.down;

    public Transform _playerPosition;
    public float Damage;
    public float HitStrength;

    Enemy enemy;
    bool _shoot = false;
    bool _isFlying = false;
    Vector2 _heading = Vector2.zero;
    Rigidbody2D RB;

    // Start is called before the first frame update
    protected virtual void Start() {
        anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();
        SpawnProjectile();
    }

    public void Initialize(EnemyAttackState attackState) {
        transform.position = attackState.AttackPosition.position;
        transform.parent = attackState.enemy.transform;
        Damage = attackState.Damage;
        HitStrength = attackState.KnockbackStrength;
        _playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        this.enemy = attackState.enemy;
    }

    protected virtual void SpawnProjectile() {

        transform.Rotate(0f, 0f, (Vector2.SignedAngle(transform.position - (transform.position + Vector3.down), transform.position - _playerPosition.position) - transform.rotation.z) * (enemy.IsFacingRight ? -1 : 1));

        Debug.Log(Vector2.SignedAngle(enemy.transform.position - (enemy.transform.position + Vector3.up), enemy.transform.position - _playerPosition.position));
        _currentDirection = (_playerPosition.position - transform.position).normalized;

        _heading = (_playerPosition.position - transform.position).normalized;
        transform.parent = null;
        _isFlying = true;
    }

    // Update is called once per frame
    protected virtual void Update() {
    }

    private void FixedUpdate() {
        if (_isFlying) {
            if (TrackPlayer) {
                var headingToPlayer = (_playerPosition.position - transform.position).normalized;
                _heading = (_heading + (Vector2)(headingToPlayer * SteeringSpeed / 100f)).normalized;
                transform.Rotate(0f, 0f, Vector2.SignedAngle(transform.position, _playerPosition.position) - transform.rotation.z);
                _currentDirection = headingToPlayer;
            }
            RB.MovePosition(transform.position + FlySpeed * (Vector3)_heading * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            damageable?.DamageWithKnockback(Damage, transform.position, HitStrength);
        }
        DestroyProjectile();
    }

    protected virtual void DestroyProjectile() {
        Destroy(this.gameObject);
    }

    public void Shoot() => _shoot = true;
}
