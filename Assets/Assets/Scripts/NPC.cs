using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : NetworkBehaviour
{
    [Header("Basics")]
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] Transform spawnTransform;
    
    public event Action OnDestinationReached;

    bool isWalking = false;
    

    void OnEnable(){
        if(spawnTransform != null){
            gameObject.transform.position = spawnTransform.position; //need to set position in menager - change constructor in Initializer
        }
        Clock.SecondPassed += CheckDestination;
    }

    void OnDisable(){
        OnDestinationReached = null;
        Clock.SecondPassed -= CheckDestination;
        navMeshAgent.ResetPath();
    }

    public void GoToDestination(Vector3 destination){
        navMeshAgent.SetDestination(destination);
        isWalking = true;
    }

    public void CheckDestination(){
        if( isWalking && HasReachedDestination()){
            navMeshAgent.ResetPath();
            OnDestinationReached?.Invoke();
            OnDestinationReached = null;
            isWalking = false;
        }
    }

    public bool HasReachedDestination(){
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    public void ResetNavmesh(){
        navMeshAgent.ResetPath();
    }

    public void TurnOffNavmesh(){
        navMeshAgent.enabled = false;
    }

    public void TurnOnNavmesh(){
         navMeshAgent.enabled = true;;
    }

}
