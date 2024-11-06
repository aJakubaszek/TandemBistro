using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField] Transform spawnTransform;
    [SerializeField] Transform mockupTable; //Zmienić na odnośnik do managera stołów z find object of type
    [SerializeField] NavMeshAgent navMeshAgent;
    TableManager tableManager;
    
    bool isSeated = false;


    void Awake(){
        navMeshAgent = GetComponent<NavMeshAgent>(); //Można zmienić na adres bezpośredni
        tableManager = FindObjectOfType<TableManager>();
    }

    void OnEnable(){
        if(spawnTransform != null){
        gameObject.transform.position = spawnTransform.position; //Wstep do poolingu
        }
        else{
            spawnTransform = gameObject.transform;
        }
        GoToTable(); // Przesunąć dalej
    }

    void OnDisable(){
        
    }

    void Update(){
        
    }

    public void NewOrder(){

    }

    public void GoToTable(){
        Table chosenTable = tableManager.GetFreeTable();
        if(chosenTable != null){
            Vector3 destination = chosenTable.GetGuestSpot();
            navMeshAgent.SetDestination(destination);
        }
        else{
            Debug.LogWarning("Client never got table");
        }

    }
}
