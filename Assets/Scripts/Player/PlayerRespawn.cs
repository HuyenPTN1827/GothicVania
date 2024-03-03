using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour {
    [SerializeField] protected Vector2 CurrentRespawnPosition;
    [SerializeField] PlayerHealth Health;
    // Start is called before the first frame update
    void Start() {
        CurrentRespawnPosition = transform.position;
        Health = GetComponent<PlayerHealth>();
    }

    public void SetRespawnPos(Vector2 pos) {
        CurrentRespawnPosition = pos;
    }

    public void Respawn(float Damage = 0f) {
        Health.Damage(Damage);
        transform.position = CurrentRespawnPosition;
    }
}
