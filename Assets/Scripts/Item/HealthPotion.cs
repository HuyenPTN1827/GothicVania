using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Item {
    [SerializeField] float HealAmount;

    public override void OnPickUp(Collision2D collision) {
        var damageable = collision.gameObject.GetComponent<IDamageable>();
        damageable?.Heal(HealAmount);

        base.OnPickUp();
    }
}
