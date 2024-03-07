using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Item {
    [SerializeField] float HealAmount;
    public AudioManager audioManager;

    public override void OnPickUp(Collision2D collision) {
        var damageable = collision.gameObject.GetComponent<IDamageable>();
        damageable?.Heal(HealAmount);
        audioManager.PlaySfx(audioManager.potionClip);
        base.OnPickUp();
    }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
}
