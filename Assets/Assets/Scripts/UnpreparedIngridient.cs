using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;

public class UnpreparedIngridient : NetworkBehaviour
{
    private NetworkVariable<int> cutCounter = new NetworkVariable<int>(0);

    private bool isCutting = false;

    [SerializeField] Transform pieceTransform;
    [SerializeField] List<GameObject> cutStates = new List<GameObject>();
    [SerializeField] GameObject cutPieces;
    

    private void OnTriggerEnter(Collider other) {
        if(!other.transform.CompareTag("Knife")){
            return;
        }

        if(cutStates.Count <= 1){
            CutInHalfServerRpc();
            return;
        }
        else{
            CutPieceServerRpc();
            return;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void CutInHalfServerRpc(){
        if(isCutting){
            return;
        }
            isCutting = true;
            CutInHalfClientRpc();
        isCutting = false;
    }

    [ClientRpc]
    private void CutInHalfClientRpc(){
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
        if(IsServer){
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutPieceServerRpc(){
        if(isCutting){
            return;
        }
        isCutting = true;
        cutCounter.Value++;
        if(cutCounter.Value+1 > cutStates.Count){
            SpawnIngredientServerRpc();
            Destroy(gameObject);
            return;
        }
        else{
            SpawnIngredientServerRpc();
            
        }
        isCutting = false;
    }

    [ServerRpc]
    private void SpawnIngredientServerRpc(){
        UpdatePieceTransformPosition();
        GameObject newIngredient = Instantiate(cutPieces, pieceTransform.position, pieceTransform.rotation);
        newIngredient.GetComponent<NetworkObject>().Spawn();
    }
    [ClientRpc]
    private void UpdateMeshClientRPC(){
        gameObject.GetComponent<MeshFilter>().mesh = cutStates[cutCounter.Value].GetComponent<MeshFilter>().mesh; 
    }

    private void UpdatePieceTransformPosition() {
        if (cutCounter.Value >= cutStates.Count) return;
        Mesh newMesh = cutStates[cutCounter.Value].GetComponent<MeshFilter>().sharedMesh;
        if (newMesh == null) return;
        Bounds bounds = newMesh.bounds;
        Vector3 rightSide = bounds.center + new Vector3(bounds.extents.x, 0, 0);
        Vector3 worldRightSide = cutStates[cutCounter.Value].transform.TransformPoint(rightSide);
        pieceTransform.position = worldRightSide;
    }
}
