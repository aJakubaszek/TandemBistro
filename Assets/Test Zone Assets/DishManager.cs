using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    public static DishManager Instance{ get; private set; }
    public List<DishData> allDishes;

    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public DishData GetRandomDish(){
            int dishID = Random.Range(0,allDishes.Count);
            return allDishes[dishID];
    }
}
