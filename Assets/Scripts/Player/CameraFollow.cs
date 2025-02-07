using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    private Vector3 offset = new Vector3(0, 0, -10f);
    private Vector3 velocity = Vector3.zero;

    [SerializeField]
    private float smoothTime = 0.25f;
    [SerializeField]
    private Transform target;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
