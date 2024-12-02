using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Dish : MonoBehaviour{
    [SerializeField] DishData data;
    
    [SerializeField] Transform topSnapTransform;
    [SerializeField] List<Ingridient> startingIngridients;
    [SerializeField] Material hoverMaterial;

    List<Transform> snappedIngridients;

    [SerializeField] XRSocketTagInteractor socketInteractor;

    void Awake(){
        socketInteractor = gameObject.GetComponent<XRSocketTagInteractor>();
        socketInteractor.targetTag = "Ingridient";
        socketInteractor.attachTransform = topSnapTransform;
        socketInteractor.interactableHoverMeshMaterial = hoverMaterial; //can't potrzebny?
    }
    
    
    void AttatchIngridient(Ingridient ingridient){
        data.ingredients.Add(ingridient.GetIngridientName());
        topSnapTransform = ingridient.GetSnapTransform();
        snappedIngridients.Add(ingridient.gameObject.GetComponent<Transform>());
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
    public DishData GetData(){
        return data;
    }

    private void LoadDish(List<Ingridient> list){
        foreach(Ingridient ing in list){

        }
    }
}
