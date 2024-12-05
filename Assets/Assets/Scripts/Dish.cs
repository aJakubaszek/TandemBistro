using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Dish : MonoBehaviour{
    [SerializeField] DishData data;
    
    [SerializeField] Transform topSnapTransform;
    [SerializeField] List<Ingridient> startingIngridients;
    [SerializeField] Material hoverMaterial;
    [SerializeField] Collider foodCollider;
    [SerializeField] Material ghostMaterial;

    List<Transform> snappedIngridients = new List<Transform>();
    private GameObject phantomObject = null;

    private void Start()
    {
        if (startingIngridients != null && startingIngridients.Count > 0){
            foreach (var ing in startingIngridients){
                AttachIngredient(ing);
            }
        }
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
            AttachIngredient(ingredient);
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
            AttachIngredient(ingredient);
        }
    }
    private void AttachIngredient(Ingridient ingredient){
       ingredient.transform.SetParent(transform);

        ingredient.transform.rotation = topSnapTransform.rotation;
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
