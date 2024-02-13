using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable {
    [SerializeField] protected float MaxHealth = 100f;
    [SerializeField] protected float CurrentHealth;

    public Rigidbody2D RB;
    public bool IsFacingRight;

    #region State Machien Variables
    public EnemyStateMachine StateMachine;
    public EnemyIdleState IdleState;
    public EnemyAggroState AggroState;
    public EnemyAttackState AttackState;
    #endregion

    void Awake() {
        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        AggroState = new EnemyAggroState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
    }

    void Start() {
        RB = GetComponent<Rigidbody2D>();
        IsFacingRight = false;

        StateMachine.Initialize(IdleState);
    }

    void Update() {
        StateMachine.CurrentState.FrameUpdate();
    }

    void FixedUpdate() {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    #region Damage/Die
    public void Damage(float damage) {
        throw new System.NotImplementedException();
    }

    public void DamageWithKnockback(float damage, Vector2 _position, float hitStrength) {
        throw new System.NotImplementedException();
    }

    public void Die() {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Animation Trigger
    private void AnimationTriggerEvent(AnimationTriggerType type) {
        StateMachine.CurrentState.AnimationTriggerEvents(type);
    }
    #endregion

    void OnDrawGizmos() {

    }

    public enum AnimationTriggerType {
        Damaged
    }
}
