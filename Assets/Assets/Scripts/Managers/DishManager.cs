using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    public static DishManager Instance{ get; private set; }
    [SerializeField] List<DishData> allDishes;

    

    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public DishData GetRandomDish(int difficulty){
            List<DishData> correctDishes = new List<DishData>();
            foreach (DishData dish in allDishes){
                if(dish.minimalLevel <= difficulty){
                    correctDishes.Add(dish);
                }
            }
            if(correctDishes.Count == 0){
                return allDishes[0];
            }
            int dishID = Random.Range(0,correctDishes.Count);
            return correctDishes[dishID];
    }
    public DishData GetRandomDish(){
            int dishID = Random.Range(0,allDishes.Count);
            return allDishes[dishID];
    }
}
