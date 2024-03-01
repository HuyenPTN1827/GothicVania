using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISummon {
    void Initialize(EnemySummonState state, Vector2 spawnLocation);
    void OnHitStart();
    void OnHitEnd();
    void OnEnemyDestroy();
    void OnAnticipateAnim();
    void OnStrikeAnim();
    void OnStrikeAnimEnd();
}
