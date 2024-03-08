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
    public Vector2 KnockbackAngle;
    public float HitStrength;

    Enemy enemy;
    bool _shoot = false;
    bool _isFlying = false;
    Rigidbody2D RB;
    Vector2 direction;

    // Start is called before the first frame update
    protected virtual void Start() {
        anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();
    }

    public void Initialize(EnemyAttackState attackState, Vector2 direction) {
        transform.position = attackState.AttackPosition.position;
        transform.parent = attackState.enemy.transform;
        //Damage = attackState.Damage;
        HitStrength = attackState.KnockbackStrength;
        _playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        this.enemy = attackState.enemy;
        this.direction = direction;

        SpawnProjectile();
    }

    protected virtual void SpawnProjectile() {

        transform.Rotate(0f, 0f, (Vector2.SignedAngle(transform.position - (transform.position + Vector3.down), direction) - transform.rotation.z) * (enemy.IsFacingRight ? -1 : 1));

        _currentDirection = (_playerPosition.position - transform.position).normalized;

        transform.parent = null;
        _isFlying = true;
    }

    // Update is called once per frame
    protected virtual void Update() {
    }

    private void FixedUpdate() {
        if (_isFlying && _shoot) {
            if (TrackPlayer) {
                //var headingToPlayer = (_playerPosition.position - transform.position).normalized;
                //_heading = (_heading + (Vector2)(headingToPlayer * SteeringSpeed / 100f)).normalized;
                //transform.Rotate(0f, 0f, Vector2.SignedAngle(transform.position, _playerPosition.position) - transform.rotation.z);
                //_currentDirection = headingToPlayer;
            }
            RB.MovePosition(transform.position - FlySpeed * (Vector3)direction * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            //Debug.Log("Damaged: " + damageable.ToString() + " damage " + Damage);
            damageable?.DamageWithKnockback(Damage, new Vector2(KnockbackAngle.x * (collision.gameObject.transform.position.x > transform.position.x ? 1 : -1), KnockbackAngle.y) , HitStrength* 100);
        }
        DestroyProjectile();
    }

    protected virtual void DestroyProjectile() {
        Destroy(this.gameObject);
    }

    public void Shoot() => _shoot = true;
}
