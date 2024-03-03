using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    [SerializeField] Vector2 pos;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] Vector2 _checkSize;
    // Start is called before the first frame update
    void Start() {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update() {
        var hits = Physics2D.OverlapBoxAll(transform.position, _checkSize, _playerLayer);
        if (hits.Length != 0) {
            foreach (var hit in hits) {
                var res = hit.gameObject.GetComponent<PlayerRespawn>();
                if (res != null) {
                    res.SetRespawnPos(transform.position);
                }
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, _checkSize);
    }
}
