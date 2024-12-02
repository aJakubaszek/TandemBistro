using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithms{
    public static List<T> ShuffleList<T>(List<T> inputList){
        for (int i = inputList.Count - 1; i > 0; i--){
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = inputList[i];
            inputList[i] = inputList[j];
            inputList[j] = temp;
        }
        return inputList;
    }

    public static void TurnOffPhysics(GameObject obj){
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if(rb != null){
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        obj.GetComponent<Collider>().enabled = false;
    }
    public static void TurnOnPhysics(GameObject obj){
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if(rb != null){
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        obj.GetComponent<Collider>().enabled = true;
    }
}
