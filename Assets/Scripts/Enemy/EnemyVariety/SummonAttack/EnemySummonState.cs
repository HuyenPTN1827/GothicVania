using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class EnemySummonState : EnemyAttackState {
    [System.Serializable]
    protected class SummonObject {
        public GameObject Object;
        [Tooltip("If randomize is selected, this will be use as maximum to calculate the number of object summoned")]
        public int count;
        public bool randomize;
        [Tooltip("AtOrigin: Object is spawned at attack origin\nTargetPlayer: Object is spawn at player location")]
        public SpawnLocation location;
    }

    [Space(20)]
    [Header("Summon Values")]
    [SerializeField] protected List<SummonObject> summonList;
    [SerializeField] protected bool SummonOnAttackAnim;


    protected List<ISummon> _summons;

    protected override void Attack() {
        base.Attack();

        if (SummonOnAttackAnim) InitializeSummons();
    }

    public override void EnterState() {
        base.EnterState();

        _summons = new List<ISummon>();
    }

    protected virtual void InitializeSummons() {
        foreach (var item in summonList) {
            var locations = new List<Vector2>();
            switch (item.location) {
                case SpawnLocation.AtOrigin:
                    locations = getOriginSummonLocation(item.Object, item.count);
                    break;
                case SpawnLocation.TargetPlayer:
                    locations = getTargetSummonLocation(item.Object, item.count);
                    break;
            }

            for (int i = 0; i < locations.Count(); i++) {
                var summon = Instantiate(item.Object);
                var summonController = summon.GetComponent<Summon>();
                summonController.Initialize(this, locations[i]);
                _summons.Add(summonController);
            }
        }
    }

    public override void HitStart() {
        foreach (var item in _summons) item.OnHitStart();

        HitEnd();
    }

    public override void HitEnd() {
        enemy.StartCoroutine(StartAttackCooldown());
        _canChangeState = true;
        foreach (var item in _summons) item.OnHitEnd();
    }

    public override void ResetValues() {
        base.ResetValues();

        _summons.Clear();
    }

    private void OnDestroy() {

        foreach (var item in _summons) item.OnEnemyDestroy();
    }

    protected virtual List<Vector2> getOriginSummonLocation(GameObject _object, int count) {
        List<Vector2> location = new List<Vector2>();

        var up = (Vector2)enemy.transform.position + Vector2.up;
        var degree = 360f / count;
        if (count == 2) degree = 90f;
        var originAngle = Vector2.SignedAngle((Vector2)enemy.transform.position - (Vector2)AttackPosition.position, (Vector2)enemy.transform.position - up);
        float degreeToA = 0f;
        if (originAngle < 0) {
            degreeToA = 180f + Mathf.Abs(180f + originAngle);
        }
        else degreeToA = originAngle;

        for (int i = 0; i < count; i++) {
            var pos = new Vector2(
                Mathf.Sin((degree * i + degreeToA) * Mathf.Deg2Rad) * Vector2.Distance(AttackPosition.position, enemy.transform.position),
                Mathf.Cos((degree * i + degreeToA) * Mathf.Deg2Rad) * Vector2.Distance(AttackPosition.position, enemy.transform.position));
            pos += (Vector2)enemy.transform.position;
            //Debug.Log(pos);
            location.Add(pos);
        }

        return location;
    }

    protected virtual List<Vector2> getTargetSummonLocation(GameObject _object, int count) {
        var maxWidth = _object.GetComponent<SpriteRenderer>().bounds.size.x * 3 / 2 + playerTransform.GetComponent<SpriteRenderer>().bounds.size.x;
        var minWidth = _object.GetComponent<SpriteRenderer>().bounds.size.x / 2 + playerTransform.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        List<Vector2> locations = new List<Vector2>();

        for (int i = 0; i < count; i++) {
            Vector2 pos = playerTransform.position;
            var hits = Physics2D.RaycastAll(pos, Vector2.down, 99f, enemy._groundLayer);
            if (hits != null && hits.Count() > 0) {
                var groundLevel = hits.OrderBy(e => e.distance).First();
                pos = new Vector2(pos.x, groundLevel.transform.position.y);
            }
            if (locations.Count == 0) {
                locations.Add(pos);
                continue;
            }
            bool isRight = false;
            int dist = 1;
            do {
                isRight = !isRight;

                pos = new Vector2(playerTransform.position.x + (Random.Range(minWidth, maxWidth) * dist * (isRight ? 1 : -1)), playerTransform.position.y);

                if (hits != null && hits.Count() > 0) {
                    var groundLevel = hits.OrderBy(e => e.distance).First();
                    pos = new Vector2(pos.x, groundLevel.transform.position.y);
                }
                if (!isRight) dist++;
            }
            while (Mathf.Abs(playerTransform.position.x - getFurthestPoint(isRight).x) > Mathf.Abs(playerTransform.position.x - pos.x));
            //Debug.Log(pos);
            locations.Add(pos);
        }

        return locations;
        Vector2 getFurthestPoint(bool isRight) {
            return locations.First(e => e.x == (isRight ? locations.Max(e => e.x) : locations.Min(e => e.x)));
        }
    }

    public enum SpawnLocation {
        AtOrigin,
        TargetPlayer
    }

}
