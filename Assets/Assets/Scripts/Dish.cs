using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Dish : MonoBehaviour{
    [SerializeField] DishData data;
    
    [SerializeField] Transform topSnapTransform;
    [SerializeField] List<Ingridient> startingIngridients;
    [SerializeField] Material hoverMaterial;
    [SerializeField] Collider foodCollider;

    List<Transform> snappedIngridients = new List<Transform>();

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
        if (ingredient != null){
            AttachIngredient(ingredient);
        }
    }

    private void AttachIngredient(Ingridient ingredient){
       ingredient.transform.SetParent(transform);

        ingredient.transform.rotation = topSnapTransform.rotation;
        data.ingredients.Add(ingredient.GetIngridientName());
        Bounds bounds = ingredient.gameObject.GetComponent<Collider>().bounds;
        //float bottomOffset = bounds.size.y/2;
        //var snapTransform = topSnapTransform;
        //Vector3 newPosition = snapTransform.position + new Vector3(0, bottomOffset, 0);
        ingredient.transform.position = topSnapTransform.position;
        ingredient.transform.rotation = topSnapTransform.rotation;

        Algorithms.TurnOffPhysics(ingredient.gameObject); 
        
        snappedIngridients.Add(ingredient.transform);

        topSnapTransform = ingredient.GetSnapTransform();
        foodCollider.transform.position = topSnapTransform.position;
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
