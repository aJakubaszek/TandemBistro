using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GuestManager : NetworkBehaviour{
    public static GuestManager Instance{ get; private set; }

    [SerializeField] int initialPoolSize;
    [SerializeField] GameObject guestPrefab;
    [SerializeField] Transform spawnPoint;
    private Queue<GameObject> customerPool = new Queue<GameObject>();

    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start(){
        if(!IsServer){gameObject.SetActive(false);}
        for(int i = 0; i <initialPoolSize; i++){
            NewGuest();
        }
    }

    public void SpawnGuest(){
        GameObject guest;
         if (customerPool.Count > 0){
            guest = customerPool.Dequeue();
            guest.SetActive(true);
        }
        else{
            NewGuest();
            SpawnGuest();
        }
        
    }

    private void NewGuest(){
        GameObject guest = Instantiate(guestPrefab, spawnPoint);
        guest.GetComponent<NetworkObject>().Spawn();
        guest.GetComponent<NPC>().spawnTransform = spawnPoint;
        guest.SetActive(false);
        customerPool.Enqueue(guest);
    }

}
