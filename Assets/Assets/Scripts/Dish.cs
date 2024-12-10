using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Dish : NetworkBehaviour{
    [SerializeField] DishData data;
    
    [SerializeField] Transform topSnapTransform;
    [SerializeField] List<Ingridient> startingIngridients;
    [SerializeField] Material hoverMaterial;
    [SerializeField] Collider foodCollider;
    [SerializeField] Material ghostMaterial;
    [SerializeField] Collider snapCollider;

    List<Transform> snappedIngridients = new List<Transform>();
    private GameObject phantomObject = null;
    
    private void OnNetworkDestroy(){
        Destroy(topSnapTransform.gameObject);
        Destroy(snapCollider.gameObject);
    }
    private void OnTriggerEnter(Collider other){
        Ingridient ingredient = other.GetComponent<Ingridient>();
        XRGrabInteractable xrGrab = other.GetComponent<XRGrabInteractable>();
        if (ingredient == null || xrGrab == null){
            return;
        }
        if(xrGrab.isSelected){
            ShowIngredientPhantom(ingredient);
        }
        else{
            DestroyPhantom();
            NewAttachServerRpc(ingredient.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    private void OnTriggerExit(Collider other) {
        XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null && grabInteractable.isSelected) {
            DestroyPhantom();
        }
    }


    private void OnTriggerStay(Collider other){
        Ingridient ingredient = other.GetComponent<Ingridient>();
        XRGrabInteractable xrGrab = other.GetComponent<XRGrabInteractable>();
        if (ingredient == null || xrGrab == null){
            return;
        }
        if(xrGrab.isSelected){
            if(phantomObject == null){
                
            }
        }
        else{
            DestroyPhantom();
            NewAttachServerRpc(ingredient.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void NewAttachServerRpc(ulong ingridientID){
        AttachIngredientClientRpc(ingridientID);
    }

    [ClientRpc(RequireOwnership = false)]
    private void AttachIngredientClientRpc(ulong ingredientID){
       NetworkObject ingredientNO = NetworkManager.Singleton.SpawnManager.SpawnedObjects[ingredientID];
       ingredientNO.TrySetParent(transform);
       Ingridient ingredient = ingredientNO.GetComponent<Ingridient>();

        ingredientNO.transform.rotation = topSnapTransform.rotation;
        data.ingredients.Add(ingredient.GetIngridientName());
        Bounds bounds = ingredient.gameObject.GetComponent<Collider>().bounds;
        ingredient.transform.position = topSnapTransform.position;
        ingredient.transform.rotation = topSnapTransform.rotation;

        Algorithms.TurnOffPhysics(ingredient.gameObject); 
        
        snappedIngridients.Add(ingredient.transform);

        topSnapTransform = ingredient.GetSnapTransform();
        foodCollider.transform.position = topSnapTransform.position;
    }

    private void ShowIngredientPhantom(Ingridient ingridient){
        if(phantomObject != null){
            return;
        }
        phantomObject = Instantiate(ingridient.gameObject, topSnapTransform.position, topSnapTransform.rotation);
        phantomObject.GetComponent<Collider>().enabled = false;
        Algorithms.TurnOnPhysics(phantomObject.gameObject);
        phantomObject.GetComponent<XRGrabInteractable>().enabled = false;

        phantomObject.transform.SetParent(transform);
    }
    private void DestroyPhantom(){
        Destroy(phantomObject);
    }
    public void SetData(DishData newData){
        data = newData;
    }

    public static bool AreDishesEqual(DishData firstDish, DishData secondDish){
        List<string> firstIngridients = firstDish.ingredients;
        List<string> secondIngridients = secondDish.ingredients;

        return firstIngridients.Count == secondIngridients.Count 
        && !firstIngridients.Except(secondIngridients).Any() 
        && !secondIngridients.Except(firstIngridients).Any();
    }
    public static bool IsOrderEqual(DishData firstDish, DishData secondDish){
        return (firstDish.ingredients.SequenceEqual(secondDish.ingredients));
    }
    public DishData GetData(){
        return data;
    }

    private void LoadDish(List<Ingridient> list){
        foreach(Ingridient ing in list){

        }
    }
}
