using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IItem {
    Collider2D Collider;
    private void Start() {
        Collider = GetComponent<Collider2D>();
    }

    public virtual void OnPickUp(Collision2D collision) {
        if (Collider != null) Collider.enabled = false;
        Destroy(gameObject);
    }

    public void OnPickUp() {
        if (Collider != null) Collider.enabled = false;
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Ground")) OnPickUp(collision);
    }
}
