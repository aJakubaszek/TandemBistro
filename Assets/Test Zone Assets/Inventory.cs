using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Transform holdingPointTransform, cameraTransform;
    [SerializeField] float maxDistance = 3f;
    GameObject heldObject = null;
    bool isHoldingItem = false;
    RaycastHit hit;
    Ray ray;
    void Start(){
    }

    void Update(){
        FindObject();
        if (Input.GetKeyDown(KeyCode.E) && hit.collider != null){ //Podzielić bloki warunkowe(?) dla optymalizacji. Przerobić na switch case z osobnymi if'ami
            if(!isHoldingItem &&   hit.collider.CompareTag("Pickup")){
                Pickup(hit.collider.gameObject);
            }
            else if(isHoldingItem && hit.collider.CompareTag("Tabletop")){
                PutDown();
            }
            else if (isHoldingItem && hit.collider.CompareTag("NPC")){
                Guest guest = hit.collider.gameObject.GetComponent<Guest>();
                if(guest != null && guest.isSeated && guest.GetOrder() != null){
                    if(guest.GiveOrder(heldObject)){
                        isHoldingItem = false;
                        Destroy(heldObject);
                    }
                }
            }
            else if(isHoldingItem && hit.collider.CompareTag("Trash")){
                TrashItem();
            }
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
                    if(guest != null && guest.isSeated && guest.GetOrder() != null){
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

    void Pickup(GameObject pickup){
        heldObject = pickup;

        Rigidbody rb = pickup.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        Collider collider = pickup.GetComponent<Collider>();
        collider.enabled = false;
        pickup.transform.position = holdingPointTransform.position;
        pickup.transform.SetParent(holdingPointTransform);

        isHoldingItem = true;
    }

    void PutDown(){
        holdingPointTransform.DetachChildren();

        float objectHeight = heldObject.GetComponent<Collider>().bounds.extents.y;

        Collider hitCollider = hit.collider;
        Vector3 hitObjectTop = hitCollider.bounds.max;

        Vector3 newPosition = new Vector3(hit.point.x, hitObjectTop.y + objectHeight, hit.point.z);
        heldObject.transform.position = newPosition;
        heldObject.transform.SetParent(hit.collider.gameObject.transform);

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        //rb.isKinematic = false;
        Collider collider = heldObject.GetComponent<Collider>();
        collider.enabled = true;

        isHoldingItem = false;
        heldObject = null;

    }

    void TrashItem(){
        Destroy(heldObject);
        isHoldingItem = false;
        heldObject = null;
    }
}
