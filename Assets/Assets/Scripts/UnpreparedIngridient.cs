using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;

public class UnpreparedIngridient : MonoBehaviour
{
    int cutCounter = 0;

    [SerializeField] Transform pieceTransform;
    [SerializeField] List<GameObject> cutStates = new List<GameObject>();
    [SerializeField] List<GameObject> cutPieces = new List<GameObject>();
    

    private void OnTriggerEnter(Collider other) {
        if(!other.transform.CompareTag("Knife")){
            return;
        }

        if(cutStates.Count < 1 && cutPieces.Count < 1){
            foreach(Transform ing in gameObject.transform){
                Ingridient ingridient = ing.GetComponent<Ingridient>();
                if(ingridient != null){
                    Rigidbody rb = ing.gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    ing.gameObject.GetComponent<BoxCollider>().enabled = true;
                    ingridient.SetPrepared(true);
                }
            }
        gameObject.transform.DetachChildren();
        Destroy(gameObject);
        }

    }
}
