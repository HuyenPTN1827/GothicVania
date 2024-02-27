using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [Header("Components")]
    public string CurrentStateName;
    public Rigidbody2D RB;
    public Collider2D Collider;
    public EnemyHealth EnemyHealth;
    public Animator Anim;
    public AIPath Path;
    public AIDestinationSetter DestinationSetter;
    public List<GameObject> Checks;

    [Space(20)]
    [Header("Check Values")]
    public bool IsFacingRight;
    public bool ApplyGravity = false;
    public Vector3 PrevPos;
    public float Speed;
    protected float _currentSpeed;

    [Space(20)]
    [Header("Ground Detection")]
    [SerializeField] protected Transform _groundCheck;
    [SerializeField] protected Vector2 _groundCheckSize;
    [SerializeField] protected Transform _frontLedgeCheck;
    [SerializeField] protected Vector2 _frontCheckSize;
    [SerializeField] protected Transform _backLedgeCheck;
    [SerializeField] protected Vector2 _backCheckSize;
    [SerializeField] protected LayerMask _groundLayer;
    float _gravityScale;

    [Space(20)]
    [Header("Player Detection")]
    [SerializeField] public float VisionRange;
    [SerializeField] public float VisionRadius;
    [SerializeField] public float AwarenessRadius;
    [SerializeField] public LayerMask _playerLayer;
    public bool DetectedPlayer;

    #region State Machine Variables
    public EnemyStateMachine StateMachine;

    [Space(20)]
    [Header("States")]
    [SerializeField] EnemyIdleState IdleState;
    [SerializeField] EnemyAggroState AggroState;
    //[SerializeField] EnemyAttackState AttackState;
    [SerializeField] List<EnemyAttackState> AttackStates;
    [SerializeField] EnemyRetreatState RetreatState;

    public EnemyIdleState IdleStateInstance { get; set; }
    public EnemyAggroState AggroStateInstance { get; set; }
    //public EnemyAttackState AttackStateInstance { get; set; }
    public EnemyRetreatState RetreatStateInstance { get; set; }
    public List<EnemyAttackState> AttackStateInstances { get; set; }

    public Dictionary<EnemyAttackState, float> OnCooldownAttacks { get; set; }
    [SerializeField] public float DelayBetweenAttacks;
    [SerializeField] public bool CanAttack;
    #endregion

    void Awake() {
        StateMachine = new EnemyStateMachine();
        RB = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        EnemyHealth = GetComponent<EnemyHealth>();
        IsFacingRight = false;
        _gravityScale = RB.gravityScale;
        Path = GetComponent<AIPath>();
        DestinationSetter = GetComponent<AIDestinationSetter>();

        AttackStateInstances = new List<EnemyAttackState>();
        OnCooldownAttacks = new Dictionary<EnemyAttackState, float>();

        IdleStateInstance = Instantiate(IdleState);
        AggroStateInstance = Instantiate(AggroState);
        //AttackStateInstance = Instantiate(AttackState);
        foreach (var attackState in AttackStates) AttackStateInstances.Add(Instantiate(attackState));
        RetreatStateInstance = Instantiate(RetreatState);
    }

    void Start() {
        Path.maxSpeed = Speed;
        if (!ApplyGravity) Path.gravity = Vector3.zero;

        CanAttack = true;

        IdleStateInstance.Initialize(gameObject, this);
        AggroStateInstance.Initialize(gameObject, this);
        //AttackStateInstance.Initialize(gameObject, this);
        foreach (var attackState in AttackStateInstances) attackState.Initialize(gameObject, this);
        RetreatStateInstance.Initialize(gameObject, this);

        StateMachine.Initialize(IdleStateInstance);

        PrevPos = transform.position;
    }

    void Update() {
        foreach (var key in OnCooldownAttacks.Keys.ToList()) {
            OnCooldownAttacks[key] -= Time.deltaTime;
            if (OnCooldownAttacks[key] <= 0) OnCooldownAttacks.Remove(key);
        }
        StateMachine.CurrentState.FrameUpdate();
        CurrentStateName = StateMachine.CurrentState.ToString();
    }

    void FixedUpdate() {
        if (Physics2D.OverlapBox(_frontLedgeCheck.transform.position, _frontCheckSize, 0f, _groundLayer) || Physics2D.OverlapBox(_backLedgeCheck.transform.position, _backCheckSize, 0f, _groundLayer)) RB.gravityScale = 0f;
        else RB.gravityScale = _gravityScale;

        StateMachine.CurrentState.PhysicsUpdate();

        //Get velocity
        _currentSpeed = Vector2.Distance(transform.position, PrevPos) / Time.fixedDeltaTime;
        PrevPos = transform.position;
        Anim.SetFloat("Speed", _currentSpeed);
    }

    public void Turn() {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
        transform.localScale.Set(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        //transform.Rotate(new Vector3(0,180, 0));
    }

    #region Animation Trigger
    private void AnimationTriggerEvent(AnimationTriggerType type) {
        StateMachine.CurrentState.AnimationTriggerEvents(type);
    }

    public void HitStart() => (StateMachine.CurrentState as EnemyAttackState)?.HitStart();
    public void HitEnd() => (StateMachine.CurrentState as EnemyAttackState)?.HitEnd();
    #endregion

    void OnDrawGizmos() {

        //Highlight Checkpoints
        foreach (var check in Checks) {
            Gizmos.DrawWireSphere(check.transform.position, 0.5f*0.4f);
        }
        //Path between checkpoints
        List<GameObject> paths = Checks.FindAll(e => e.name.Contains("patrol"));
        for (int i = 0; i < paths.Count - 1; i++) {
            Gizmos.DrawLine(paths[i].transform.position, paths[i + 1]?.transform.position ?? paths[i].transform.position);
        }

        //ground checks
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontLedgeCheck.position, _frontCheckSize);
        Gizmos.DrawWireCube(_backLedgeCheck.position, _backCheckSize);
        Gizmos.color = Color.white;

        if (StateMachine?.CurrentState != null) StateMachine.CurrentState.OnDrawGizmos();
    }

    private void OnDestroy() {
        IdleStateInstance.EnemyKilled();
        AggroStateInstance.EnemyKilled();
        //AttackStateInstance.EnemyKilled();
        foreach (var attack in AttackStateInstances) attack.EnemyKilled();
    }

    public enum AnimationTriggerType {
        Damaged
    }
}
