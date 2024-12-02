using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Dish : MonoBehaviour{
    [SerializeField] DishData data;
    
    [SerializeField] Transform topSnapTransform;
    [SerializeField] string ingridientName;
    [SerializeField] Material hoverMaterial;

    List<Transform> snappedIngridients;

    XRSocketTagInteractor socketInteractor;

    void Awake(){
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

    public static Dish LoadDishFromData(DishData newData){
        Dish newDish = new Dish();
        newDish.SetData(newData);
        return newDish;
    }

    public static bool AreDishesEqual(Dish firstDish, Dish secondDish){
        List<string> firstIngridients = firstDish.data.ingredients;
        List<string> secondIngridients = secondDish.data.ingredients;

        return firstIngridients.Count == secondIngridients.Count 
        && !firstIngridients.Except(secondIngridients).Any() 
        && !secondIngridients.Except(firstIngridients).Any();
    }
}
