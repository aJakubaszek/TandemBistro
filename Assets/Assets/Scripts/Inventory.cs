using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    public Transform holdingPointTransform, cameraTransform, parentTransform;
    [SerializeField] float maxDistance = 3f;
    NetworkObject heldObject = null;
    bool isHoldingItem = false;
    RaycastHit hit;
    Ray ray;

    void Update(){
        if(!IsOwner) {return;}
        FindObject();
        if (Input.GetKeyDown(KeyCode.E) && hit.collider != null){ //Podzielić bloki warunkowe(?) dla optymalizacji. Przerobić na switch case z osobnymi if'ami
            if(!isHoldingItem && hit.collider.CompareTag("Pickup")){
                RequestPickupServerRpc(hit.collider.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
            }
            else if(isHoldingItem && hit.collider.CompareTag("Tabletop")){
                RequestPutDownServerRpc();
            }
            else if (isHoldingItem && hit.collider.CompareTag("NPC")){
                Guest guest = hit.collider.gameObject.GetComponent<Guest>();
                if(guest != null && guest.isSeated.Value && guest.isWaiting.Value){
                    NetworkObject networkObject = heldObject.GetComponent<NetworkObject>();
                    guest.GiveOrderServerRpc(networkObject.NetworkObjectId);
                    
                }
            }
            else if(isHoldingItem && hit.collider.CompareTag("Trash")){
                TrashItemClientRpc();
            }
        }

    else if (Input.GetKeyDown(KeyCode.Q)){
        RequestThrowServerRpc();
    }
    }


    void FindObject(){
        ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit newHit, maxDistance)){
            if (newHit.collider != hit.collider){
                hit = newHit;
                if(!isHoldingItem && hit.collider.CompareTag("Pickup")){
                    CursorManager.Instance.SetCursor(CursorManager.Instance.GetPickupCursor());
                }
                else if(isHoldingItem && hit.collider.CompareTag("Tabletop")){
                    CursorManager.Instance.SetCursor(CursorManager.Instance.GetPutDownCursor());
                }
                else if(isHoldingItem && hit.collider.CompareTag("Trash")){
                    CursorManager.Instance.SetCursor(CursorManager.Instance.GetTrashCursor());
                }
                else if(isHoldingItem && hit.collider.CompareTag("NPC")){
                    Guest guest = hit.collider.gameObject.GetComponent<Guest>();
                    if(guest != null && guest.isSeated.Value && guest.isWaiting.Value){
                        CursorManager.Instance.SetCursor(CursorManager.Instance.GetGiveCursor());
                    }
                }
                else{
                    CursorManager.Instance.ResetCursor();
                    hit = newHit;
                }
            }
        }
        else{
            CursorManager.Instance.ResetCursor();
            hit = default;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void RequestPickupServerRpc(ulong pickupID){
        PickupClientRpc(pickupID);
    }

    [ClientRpc(RequireOwnership = false)]
    void PickupClientRpc(ulong pickupID){
        Debug.Log("pickup");
        heldObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[pickupID];

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if(rb != null){
        rb.isKinematic = true;
        }
        Collider collider = heldObject.GetComponent<Collider>();
        if(collider != null){
        collider.enabled = false;
        }
        heldObject.transform.position = holdingPointTransform.position;

        heldObject.TrySetParent(parentTransform);

        isHoldingItem = true;
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestPutDownServerRpc(){
        PutDownClientRpc();
    }
     
    [ClientRpc(RequireOwnership = false)]
    void PutDownClientRpc(){
        LeaveHeldItemClientRpc();

        float objectHeight = heldObject.GetComponent<Collider>().bounds.extents.y;

        Collider hitCollider = hit.collider;
        Vector3 hitObjectTop = hitCollider.bounds.max;

        Vector3 newPosition = new Vector3(hit.point.x, hitObjectTop.y + objectHeight, hit.point.z);
        heldObject.transform.position = newPosition;
        heldObject.transform.SetParent(hit.collider.gameObject.transform);

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        Collider collider = heldObject.GetComponent<Collider>();
        collider.enabled = true;


    }
    [ServerRpc(RequireOwnership = false)]
    void RequestThrowServerRpc(){
        ThrowItemClientRpc();
    }

    [ClientRpc(RequireOwnership = false)]
    void ThrowItemClientRpc(){
        if(heldObject != null){
            NetworkObject obj = heldObject;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if(rb != null){
                LeaveHeldItemClientRpc();
            
                Vector3 throwDirection = (cameraTransform.forward + Vector3.up).normalized;
                rb.isKinematic = false;
                obj.GetComponent<Collider>().enabled = true;
                rb.velocity = throwDirection * 7;
            }
            else{
                Debug.LogWarning("Held object has no RiggidBody");
            }
            
        }
    }


    [ClientRpc(RequireOwnership = false)]
    void LeaveHeldItemClientRpc(){ //nie ustawione
        heldObject.TryRemoveParent();
        heldObject = null;
        isHoldingItem = false;
    }
    [ClientRpc(RequireOwnership = false)]
    public void TrashItemClientRpc(){ //nie ustawione
        LeaveHeldItemClientRpc();
        Destroy(heldObject);
    }
}
