using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Ingridient : MonoBehaviour
{
    [SerializeField] Transform topSnapTransform;
    [SerializeField] string ingridientName;
    [SerializeField] bool isPrepared = false;


    public string GetIngridientName(){
        return ingridientName;
    }

    public Transform GetSnapTransform(){
        return topSnapTransform;
    }

    public void SetPrepared(bool prepared){
        isPrepared = prepared;
    }
}
