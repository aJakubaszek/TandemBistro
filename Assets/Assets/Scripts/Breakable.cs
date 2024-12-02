using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] float breakSpeed = 5f;
    [SerializeField] AudioClip breakSound;
    Rigidbody rb;

    void Start(){
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other) {
        if(rb.velocity.magnitude > breakSpeed){
        gameObject.transform.DetachChildren();
        if (breakSound != null){
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }
        Destroy(gameObject);
        }
    }
}
