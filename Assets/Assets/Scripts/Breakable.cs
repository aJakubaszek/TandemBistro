using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {
    [SerializeField] float breakSpeed = 5f;
    [SerializeField] AudioClip breakSound;
    Rigidbody rb;

    void Start() {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other) {
        if (rb.velocity.magnitude > breakSpeed) {
            foreach (Transform piece in gameObject.transform) {
                Rigidbody rb = piece.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody; //zmieniæ na
                MeshCollider mc = transform.gameObject.GetComponent<MeshCollider>(); //zmieniæ na try - nie ka¿dy to ma
                if (mc != null) {
                    mc.enabled = true;
                }
            }
            gameObject.transform.DetachChildren();
            if (breakSound != null) {
                AudioSource.PlayClipAtPoint(breakSound, transform.position);
            }
            Destroy(gameObject);
        }
    }
}