using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RangeAttackState : EnemyAttackState {
    [Space(20)]
    [Header("Range Values")]
    [SerializeField] protected GameObject Projectile;
    [SerializeField] protected float SpawnDelayTime;

    GameObject _projectile;


    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);

        
    }
    protected override void Attack() {
        base.Attack();

        _projectile = Instantiate(Projectile);
        var projectileController = _projectile.GetComponent<EnemyProjectile>();
        projectileController.Initialize(this);
        projectileController.Shoot();

        
        HitEnd();
    }
}
