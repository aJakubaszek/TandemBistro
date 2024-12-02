using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    
    [SerializeField] List<Transform> seats;
    [SerializeField] Transform meetingPointTransform;
    public event Action<Table,bool> tableStatusChanged;
    public bool isOccupied { get; private set; } = false;



    public void SeatGuests(List<Transform> guests){
        seats = Algorithms.ShuffleList<Transform>(seats);
        for(int i = 0; i < guests.Count -1; i++){
            guests[i].position = seats[i].position;
        }
        isOccupied = false;
    }
    public void SeatGuests(Transform guest){
        seats = Algorithms.ShuffleList<Transform>(seats);
        guest.position = seats[0].position;

    }
    public Vector3 GetGuestSpot(){
        return meetingPointTransform.position;
    }
    private void RemoveGuests(){
        isOccupied = true; //add the rest
    }

     public void ChangeTableStatus(){
        isOccupied = !isOccupied;
        tableStatusChanged?.Invoke(this,isOccupied);
    }
}
