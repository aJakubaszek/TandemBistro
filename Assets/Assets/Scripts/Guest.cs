using System.Collections;
using Unity.Netcode;
using UnityEngine;

[GenerateSerializationForTypeAttribute(typeof(DishData))]
public class Guest : NetworkBehaviour
{
    [Header("Difficulty Settings")]
    [SerializeField] float orderTime = 5f;
    [SerializeField] float eatingTime = 3f;
    [SerializeField] float patience = 60f;
    [SerializeField] UnityEngine.UI.Image orderImage;

    public NPC movement;
    Table table;
    public NetworkVariable<bool> isSeated = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isWaiting = new NetworkVariable<bool>(false);
    public NetworkVariable<float> timeWaiting = new NetworkVariable<float>(0);
    DishData order = new DishData();

    Collider col;
    void Start(){
        Debug.Log("Starting2" + IsServer);
        if(!IsServer){return;}
        movement = gameObject.GetComponent<NPC>();
        col = gameObject.GetComponent<Collider>();

        GetTable(); //to potem wyrzucić - Debug only
    }
    void OnEnable(){
        if(!IsServer){return;}
        GetTable();
    }
    void OnDisable(){
        table = null;
        order = new DishData();
        isSeated.Value = false;
        isWaiting.Value = false;
        timeWaiting.Value = 0;
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
            Debug.Log("Siadanie - prepare");
            movement.GoToDestination(destination);
            if(isWaiting.Value){
                Clock.SecondPassed -= WaitMode;
                isWaiting.Value = false;
                timeWaiting.Value = 0;
            }
        }
        else{
            if(!isWaiting.Value){
                Transform waitingSpot = TableManager.Instance.GetWaitingPoint();
                movement.OnDestinationReached += (()=> Clock.SecondPassed += WaitMode); //Overload GoToDestination with action argument?
                movement.GoToDestination(waitingSpot.position);
            }
        }
    }

    private void TakeYourSeat(){ 
        movement.TurnOffNavmesh();
        table.SeatGuests(gameObject.transform);
        float height = gameObject.GetComponent<Collider>().bounds.size.y;
        transform.position += new Vector3(0, height / 2, 0);

        if(IsServer){
            isSeated.Value = true;
            order = DishManager.Instance.GetRandomDish(LevelManager.Instance.GetDifficulty());
            Clock.SecondPassed += WaitMode;
            DisplayOrderClientRpc(order.dishImageID);
        }
        
    }

    [ServerRpc(RequireOwnership = false)] 
    public void GiveOrderServerRpc(ulong networkId){
        NetworkObject plate = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkId];
        Dish givenDish = plate.GetComponent<Dish>();
        bool isCorrectOrder = Dish.AreDishesEqual(givenDish.GetData(), order);
        if(givenDish != null && isCorrectOrder){
            if(LevelManager.Instance == null) {return;}
            Clock.SecondPassed -= WaitMode;
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            movement.TurnOnNavmesh();
            HideOrderClientRpc();
            LevelManager.Instance.NewGuestServed(RateDish(givenDish));
            foreach(var player in FindObjectsOfType<Inventory>()){
                player.TrashItemClientRpc();
            }
            
        }
        else{
            Debug.Log("Niepoprawne zamówienie");
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            if(LevelManager.Instance == null) {return;}
            LevelManager.Instance.GuestLeavesUnserved();
        }
    }

    private void WaitMode(){
        if(!isWaiting.Value){ isWaiting.Value = true;} //Change if-blocks to state machine
        if(!isSeated.Value){ GetTable(); }
        timeWaiting.Value += 1;
        if(timeWaiting.Value > patience){
            Clock.SecondPassed -= WaitMode;
            timeWaiting.Value = 0;
            isWaiting.Value = false;
            Debug.Log("waiting too long");
            if(isSeated.Value){
                isSeated.Value = false;
                gameObject.transform.position = table.GetGuestSpot();
                movement.TurnOnNavmesh();
                table.ChangeTableStatus();
                table = null;
                order = new DishData();
                HideOrderClientRpc();
            }
            movement.OnDestinationReached += ()=> {gameObject.SetActive(false);}; //Zmienić na punkt na zewnatrz
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

    [ClientRpc]
    void DisplayOrderClientRpc(int newImageID){
        if(orderImage != null){
            orderImage.sprite = SpriteManager.Instance.GetSprite(newImageID);
            Color cc = orderImage.color;
            orderImage.color = new Color(cc.r,cc.g,cc.b, 1);
        }
    }

    [ClientRpc]
    void HideOrderClientRpc(){
        if(orderImage != null){
            orderImage.sprite = null;
            Color cc = orderImage.color;
            orderImage.color = new Color(cc.r,cc.g,cc.b, 0);
        }
    }

    private int RateDish(Dish givenDish){
        int points = 0;
        float multiplier = 0;
        foreach (Transform foodTransform in givenDish.snappedIngridients){
            GameObject foodGO = foodTransform.gameObject;
            if(foodGO.GetComponent<Ingridient>() != null){
                points += 10;
            }
            TwoSidedMeat meat = foodGO.GetComponent<TwoSidedMeat>();
            if(meat != null){
                points += 5;
                multiplier += meat.GetModifier();
            }
        }

        return (int)(points*multiplier);
    }
}
