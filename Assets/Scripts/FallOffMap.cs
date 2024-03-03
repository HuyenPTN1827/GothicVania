using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOffMap : MonoBehaviour {
    [SerializeField] GameObject Player;
    PlayerHealth Health;

    private void Start() {
        Health = Player.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update() {
        if (Player.transform.position.y < transform.position.y) {
            Health?.Die();
        }
    }
}
