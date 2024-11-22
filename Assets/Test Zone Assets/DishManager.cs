using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    public static DishManager Instance{ get; private set; }
    public List<Dish> allDishes;

    private void Awake(){ //Add loading dishes from json
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Dish GetRandomDish(){
        if(allDishes.Count >0){
            int dishID = Random.Range(0,allDishes.Count);
            return allDishes[dishID];
        }
        else{
            return null;
        }
    }
}
