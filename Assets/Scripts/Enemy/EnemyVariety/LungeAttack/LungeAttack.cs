using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu]
public class LungeAttack : EnemyAttackState {
    public AudioManager audioManager;

    [Space(20)]
    [Header("Lunge Values")]
    [SerializeField] float _lungeSpeedMultiplier;
    [SerializeField] float _anticipationSpeedMultiplier;

    Collider2D _playerCollider;
    bool _isAttacking;
    Vector2 _lungeVector;

    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);

        _playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();    
    }

    public override bool IsInRangeForAttack() => Physics2D.CircleCastAll(AttackPosition.position, AttackRange / 2, -enemy.transform.right * (enemy.IsFacingRight ? - 1: 1), enemy.VisionRange, enemy._playerLayer).Length > 0;

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();

        if (_isAttacking) {
            enemy.RB.velocity = _lungeVector;
        }
    }

    protected override RaycastHit2D[] GetHits() => Physics2D.CircleCastAll(enemy.transform.position, AttackRange / 2, Vector2.zero, 0f, enemy._playerLayer);

    public override void EnterState() {
        base.EnterState();

        enemy.Speed *= _anticipationSpeedMultiplier;
    }

    public override void ResetValues() {
        base.ResetValues();

        enemy.Speed /= _anticipationSpeedMultiplier;
    }

    protected override void Attack() {
        base.Attack();


        enemy.DestinationSetter.target = null;
        enemy.Path.canMove = false;
        enemy.DestinationSetter.enabled = false;
        enemy.RB.velocity = Vector2.zero;
        audioManager.PlaySfx(audioManager.attackClip);

        _lungeVector = (playerTransform.position - enemy.transform.position).normalized * _lungeSpeedMultiplier;
    }

    public override void HitStart() {
        base.HitStart();

        Physics2D.IgnoreCollision(enemy.Collider, _playerCollider, true);

        var target = enemy.DestinationSetter.target;


        _isAttacking = true;
    }

    public override void OnDrawGizmos() {
        base.OnDrawGizmos();

        var direction = -enemy.transform.right * (enemy.IsFacingRight ? -1 : 1) * enemy.Speed * _lungeSpeedMultiplier* 2f;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(AttackPosition.position, AttackPosition.position + direction);
    }

    public override void HitEnd() {
        base.HitEnd();

        Physics2D.IgnoreCollision(enemy.Collider, _playerCollider, false);

        enemy.RB.velocity = Vector2.zero;

        enemy.Path.canMove = true;
        enemy.DestinationSetter.enabled = true;

        _isAttacking = false;   
    }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
}
