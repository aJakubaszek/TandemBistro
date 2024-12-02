using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public static Clock Instance{get; private set;}
    public static event Action SecondPassed;
    public static event Action HalfSecondPassed;

    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad???
    }

    private void Start(){
        StartCoroutine(CounterOneSecond());
        StartCoroutine(CounterHalfSecond());
    }

    private IEnumerator CounterOneSecond(){
        while(true){
            yield return new WaitForSeconds(1);
            SecondPassed?.Invoke();
        }
    }

    private IEnumerator CounterHalfSecond(){
        while(true){
            yield return new WaitForSeconds(0.5f);
            HalfSecondPassed?.Invoke();
        }
    }
}
