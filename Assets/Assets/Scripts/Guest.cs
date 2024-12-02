using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guest : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [SerializeField] float orderTime = 5f;
    [SerializeField] float eatingTime = 3f;
    [SerializeField] float patience = 60f;

    NPC movement;
    Table table;
    DishData order;
    
    public bool isSeated = false; //TBD: State Machine
    public bool isWaiting = false;
    public float timeWaiting = 0;

    void Awake(){
        movement = gameObject.GetComponent<NPC>();
        
    }
    void OnEnable(){

        GetTable();
    }
    void OnDisable(){
        table = null;
        order = new DishData();
        isSeated = false;
        isWaiting = false;
        timeWaiting = 0;
        Clock.SecondPassed -= WaitMode;
    }

    private void GetTable(){
        if (TableManager.Instance == null){
            StartCoroutine(WaitForTableManager());
            Debug.Log("Waiting for Table Manager");
            return;
        }

        table = TableManager.Instance.GetFreeTable();
        if(table != null){
            Vector3 destination = table.GetGuestSpot();
            movement.OnDestinationReached += TakeYourSeat;
            movement.GoToDestination(destination);
            if(isWaiting){
                Clock.SecondPassed -= WaitMode;
                isWaiting = false;
                timeWaiting = 0;
            }
        }
        else{
            if(!isWaiting){
                Transform waitingSpot = TableManager.Instance.GetWaitingPoint();
                movement.OnDestinationReached += (()=> Clock.SecondPassed += WaitMode); //Overload GoToDestination with action argument?
                movement.GoToDestination(waitingSpot.position);
            }
        }
    }

    private void TakeYourSeat(){ 
        Debug.Log("Trying to seat down");//await z myśleniem?
        
        table.SeatGuests(gameObject.transform);
        isSeated = true;
        order = DishManager.Instance.GetRandomDish();
        Clock.SecondPassed += WaitMode;
        //Display dish image
        
    }

    public bool GiveOrder(GameObject plate){//adding points
        Dish givenDish = plate.GetComponent<Dish>();
        bool isCorrectOrder = Dish.AreDishesEqual(givenDish.GetData(), order);
        if(givenDish != null && isCorrectOrder){
            Clock.SecondPassed -= WaitMode;
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            return true;
        }
        else{
            Debug.Log("Niepoprawne zamówienie");
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            return false;
        }
    }

    private void WaitMode(){
        if(!isWaiting){ isWaiting = true;} //Change if-blocks to state machine
        if(!isSeated){ GetTable(); }
        timeWaiting++;
        if(timeWaiting > patience){
            Clock.SecondPassed -= WaitMode;
            timeWaiting = 0;
            isWaiting = false;
            Debug.Log("waiting too long");
            if(isSeated){
                isSeated = false;
                gameObject.transform.position = table.GetGuestSpot();
                table.ChangeTableStatus();
                table = null;
                order = new DishData();
            }
            movement.OnDestinationReached += (()=> {gameObject.SetActive(false);}); //Zmienić na punkt na zewnatrz
            Transform exitPoint = TableManager.Instance.GetWaitingPoint();
            movement.GoToDestination(exitPoint.position);
        }

    }
    private void NewOrder(){
       order = DishManager.Instance.GetRandomDish();
    }

    public DishData GetOrder(){
        return order;
    }

    private IEnumerator WaitForTableManager(){
        while(TableManager.Instance == null){
            yield return null;
        }
        GetTable();
    }


}
