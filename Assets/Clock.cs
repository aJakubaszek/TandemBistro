using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public static Clock instance{get; private set;}
    public event Action secondPassed;

    private void Awake(){
        if (instance != null && instance != this){
            Destroy(this);
        }
        else{
             instance = this;
        }
    }

    private IEnumerator counterOneSecond(){

        yield return new WaitForSeconds(1);
        secondPassed?.Invoke();

    }

}
