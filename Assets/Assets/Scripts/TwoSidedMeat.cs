using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class TwoSidedMeat : NetworkBehaviour
{
    [SerializeField] Collider bottomCollider;
    [SerializeField] Collider topCollider;
    [SerializeField] float cookingTime = 15f;
    public NetworkVariable<float> topSecondsCooked = new NetworkVariable<float>(0f);
    public NetworkVariable<float> bottomSecondsCooked = new NetworkVariable<float>(0f);
    Collider touchedOven;
    bool isCooking = false;


 
    Material topMaterial;
    Material bottomMaterial;

    [SerializeField] GameObject top;
    [SerializeField] GameObject bottom;



    void Awake(){
       float multiplier = Random.Range(0.8f, 1.2f);
       cookingTime *= multiplier;
       topMaterial = top.GetComponent<Renderer>().material;
       bottomMaterial = bottom.GetComponent<Renderer>().material;
    }


    private void OnTriggerEnter(Collider other){
        if(!IsServer){return;}
        if(other.CompareTag("Oven")){
             touchedOven = other;
             isCooking = true;
             Clock.HalfSecondPassed += CookSide;
        }
    }

    private void OnTriggerExit(Collider other){
        if(!IsServer){return;}
        if(other.CompareTag("Oven")){
            touchedOven = null;
            isCooking = false;
            Clock.HalfSecondPassed -= CookSide;
        }
    }
    void CookSide(){
        if(touchedOven != null){
            if(isCooking){
                if(bottomCollider.bounds.Intersects(touchedOven.bounds)){
                    bottomSecondsCooked.Value += 0.5f;
                    UpdateTexture(bottomMaterial, bottomSecondsCooked.Value);
                }
                else if (topCollider.bounds.Intersects(touchedOven.bounds)){
                    topSecondsCooked.Value += 0.5f;
                    UpdateTexture(topMaterial, topSecondsCooked.Value);
                }
            }
        }
    }

    
    void UpdateTexture(Material updatedMaterial, float secondsCooked){
        float percent = Mathf.Clamp(secondsCooked / cookingTime, 0f, 1f);
        Color startingColor = new Color(1f, 0.49f, 0.49f);
        Color cookedColor = new Color(0.43f, 0.30f, 0.30f); 
        Color burntColor = Color.black;

        Color color;
        if(percent < 1){
        color = Color.Lerp(startingColor, cookedColor, percent);
        }
        else{
        color = Color.Lerp(cookedColor, burntColor, Mathf.Clamp(((secondsCooked / (cookingTime)) - 1f)*2, 0f, 1f));
        }

        updatedMaterial.color = color;
    }
}
