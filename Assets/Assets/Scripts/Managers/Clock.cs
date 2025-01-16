using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Clock : NetworkBehaviour{
    public static Clock Instance{get; private set;} //Destroy dla gracza w starcie?
    public static event Action SecondPassed;
    public static event Action HalfSecondPassed;
    public float seccondsPassed = 0;

    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start(){
        if(!IsHost){return;}
        StartCoroutine(CounterOneSecond());
        StartCoroutine(CounterHalfSecond());
    }

    private IEnumerator CounterOneSecond(){
        while(true){
            yield return new WaitForSeconds(1);
            SecondPassed?.Invoke();
            seccondsPassed++;
        }
    }

    private IEnumerator CounterHalfSecond(){
        while(true){
            yield return new WaitForSeconds(0.5f);
            HalfSecondPassed?.Invoke();
        }
    }
}
