using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] float breakSpeed = 5f;
    [SerializeField] AudioClip breakSound;
    Rigidbody rb;
    bool plateBroken = false;

    void Start(){
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other) {
        if (plateBroken){
            return;
        }
        if(other.gameObject.GetComponent<Player>()){
            return;
        }
        if(rb.velocity.magnitude > breakSpeed){
            plateBroken = true;
            Debug.Log($"Object velocity: {rb.velocity.magnitude}");
            foreach(Transform piece in gameObject.transform){
                    Rigidbody rbPiece = piece.gameObject.GetComponent<Rigidbody>();
                    if(rbPiece != null){
                        rbPiece.useGravity = true;
                        rbPiece.isKinematic = false;

                        Vector3 randomDirection = new Vector3(
                        Random.Range(-1f, 1f), // X
                        Random.Range(0.5f, 1f), // Y
                        Random.Range(-1f, 1f)  // Z
                        ).normalized;
                        float randomForce = Random.Range(0f,4f);

                        rb.AddForce(randomDirection * randomForce, ForceMode.Impulse);
                    }

                    Collider mc = piece.gameObject.GetComponent<Collider>();
                    if(mc != null){
                        mc.enabled = true;
                    }
            }
            gameObject.transform.DetachChildren();
            if (breakSound != null){
                AudioSource.PlayClipAtPoint(breakSound, transform.position);
            }
            Destroy(gameObject);
        }
    }
}
