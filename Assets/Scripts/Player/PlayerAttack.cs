using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour {
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private PlayerMovement _playerMovement;

    [SerializeField] private float _attackTimer = 0f;

    [Header("Layer & Tags")]
    [SerializeField] private LayerMask _monsterLayer;
    [SerializeField] private Transform _attackTransform;

    [Header("Attack values")]
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _playerDamage = 1f;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private bool _hitKnockback = false;
    [SerializeField] private float _hitStrength = 100f;
    [SerializeField] private string _attackName;

    private List<IDamageable> _damageables;
    private bool _isAttacking = false;

    // Start is called before the first frame update
    void Start() {
        _damageables = new List<IDamageable>();
        _animator = GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (_attackTimer > 0f) {
            _attackTimer -= Time.fixedDeltaTime;
        }
        else _attackTimer = 0f;

    }

    public void OnAttack(InputAction.CallbackContext context) {
        if (_attackTimer == 0f) {
            if (context.started) StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack() {
        _animator.SetTrigger("Attack");
        while (!_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(_attackName)) {
            _attackTimer = 1f;
            yield return null;
        }
        foreach (var clip in _animator.GetCurrentAnimatorClipInfo(0)) if (clip.clip.name.Equals(_attackName)) _attackTimer = clip.clip.length;
    }

    private IEnumerator StartHitting() {
        var position = transform.position;
        while (_isAttacking) {
            var hits = Physics2D.CircleCastAll(_attackTransform.position, _attackRange, transform.right, 0f, _monsterLayer);
            foreach (var hit in hits) {
                if (hit == null) continue;
                IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                if (!_damageables.Contains(damageable) && damageable != null) _damageables.Add(damageable);
                if (!_playerMovement.IsDashing) ExecuteHit();
            }
            yield return null;
        }
        ExecuteHit(position);
    }

    private void ExecuteHit(Vector2? position = null) {
        foreach (var damageable in _damageables) {
            if (damageable != null) {
                Debug.Log("hit");
                if (_hitKnockback) damageable?.DamageWithKnockback(_playerDamage, position??transform.position, CalculateHitStrength());
                else damageable?.Damage(_playerDamage);
            }
        }
        _isAttacking = false;
        _damageables.Clear();
        _attackTimer = _attackCooldown;
    }

    private float CalculateHitStrength() => _hitStrength;

    private void ShouldRecordHit() {
        _isAttacking = true;
        StartCoroutine(nameof(StartHitting));
    }
    private void ShouldStopRecordHit() => _isAttacking = false;

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(_attackTransform.position, _attackRange);
    }
}
