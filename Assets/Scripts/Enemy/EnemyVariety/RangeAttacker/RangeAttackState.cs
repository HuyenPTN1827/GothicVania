using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RangeAttackState : EnemyAttackState {
    [Space(20)]
    [Header("Range Values")]
    [SerializeField] protected GameObject Projectile;
    [SerializeField] protected float SpawnDelayTime;
    [SerializeField] protected bool HasSpawnWhineUp;
 
    GameObject _projectile;


    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);
        
    }
    protected override void Attack() {
        base.Attack();

        if (HasSpawnWhineUp) InitiateProjectile();
    }

    protected virtual void InitiateProjectile() {
        _projectile = Instantiate(Projectile);
        var projectileController = _projectile.GetComponent<EnemyProjectile>();
        projectileController.Initialize(this, (AttackPosition.position - playerTransform.position).normalized);
    }

    public override void HitStart() {
        if (_projectile == null) InitiateProjectile();
        var projectileController = _projectile.GetComponent<EnemyProjectile>();
        projectileController.Shoot();


        HitEnd();
    }

    public override void HitEnd() {
        enemy.StartCoroutine(StartAttackCooldown());
        _canChangeState = true;
    }

    public override void ExitState() {
        base.ExitState();
        
        _projectile = null;
    }

}
