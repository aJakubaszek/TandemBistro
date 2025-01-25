using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance;
    public event Action<int> GuestServed;
    public event Action GuestFailed;
    public event Action LevelComplete;
   [SerializeField] LevelData levelData;
    [SerializeField] TextMeshPro screenCounter;
   [SerializeField] int timeFromLastGuest = 0;
    [SerializeField] int guestsServed = 0;
    [SerializeField] int guestsServedPoorly = 0;
    [SerializeField] int guestsSpawned = 0;
    int points = 0;
    [SerializeField] float guestSpawnChance = 0;
    [SerializeField] float currentChance = 0;
    [SerializeField] float randomValue = 0;
   [SerializeField]  int levelTime = 0;


    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start(){
        if(!IsServer){gameObject.SetActive(false);}
        //Subscribe to guest manager
    }

    public void StartLevel(LevelData data){
        if (data == null || !IsServer){return;}
        levelData = data;
        guestSpawnChance = 1/((float)levelData.maxTime - (float)levelData.minTime);
        levelData = data;
        randomValue = UnityEngine.Random.value;
        
        Clock.SecondPassed += IncrementCounter;
        Clock.SecondPassed += TryAddGuest;
    }
    public void StopLevel(){
        levelData = null;
        timeFromLastGuest = 0;
        guestsServed = 0;
        guestsServedPoorly = 0;
        guestsSpawned = 0;
        guestSpawnChance = 0;
        currentChance = 0;
        randomValue = 0;
        levelTime = 0;
        points = 0;

        Clock.SecondPassed -= IncrementCounter;
        Clock.SecondPassed -= TryAddGuest;


        //Wysłać event że skończone [jednak zrobić na samej górze - zależnie czy win czy lost]
        //Wyświetlić panel wyników
    }
    public void TryAddGuest(){
        if(guestsSpawned >= levelData.maxGuests){return;}
        if(timeFromLastGuest >= levelData.minTime){
            currentChance += guestSpawnChance;
            if(currentChance >= randomValue){
                GuestManager.Instance.SpawnGuest();
                currentChance = 0;
                timeFromLastGuest = 0;
                guestsSpawned++;
                randomValue = UnityEngine.Random.value;
                
            }
        }
    }
    public void NewGuestServed(int p){
        guestsServed++;
        points += p;
        if(guestsServed >= levelData.maxGuests){
            StopLevel();
            Debug.Log("Wygrana");
        }
    }

    public void GuestLeavesUnserved(){
        guestsServedPoorly++;
        if(guestsServedPoorly >= levelData.maxMadGuests){
            StopLevel();
            Debug.Log("Przegrana");
        }
    }
    public void IncrementCounter(){
        levelTime++;
        timeFromLastGuest++;
        if(screenCounter != null){
            screenCounter.text = levelTime.ToString();
        }
    }
}
