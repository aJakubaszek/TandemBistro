using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Breakable : NetworkBehaviour
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
            RequestBreakServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestBreakServerRpc(){
        if(IsServer){
            plateBroken = true;
            BreakClientRpc();
        }

    }
    [ClientRpc(RequireOwnership = false)]
    void BreakClientRpc(){
        plateBroken = true;
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
            for( int i = 0; i < gameObject.transform.childCount; i++){
                NetworkObject NO = gameObject.transform.GetChild(0).GetComponent<NetworkObject>();
                if(NO != null && IsServer){
                    Debug.Log("break");
                    NO.TryRemoveParent();
                }
                else{
                    if(IsServer){
                        Debug.Log(gameObject.transform.GetChild(i).name);
                        gameObject.transform.GetChild(i).SetParent(null);
                    }
                }
            }
            if (breakSound != null){
                AudioSource.PlayClipAtPoint(breakSound, transform.position);
                Debug.Log("dzwiek");
            }
            if(IsServer){
                gameObject.GetComponent<NetworkObject>().Despawn();
                
            }
            Destroy(gameObject);
    }
}
