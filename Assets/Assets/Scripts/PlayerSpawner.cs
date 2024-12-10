using Unity.Netcode;
using UnityEngine;

public class CustomPlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Start(){
        if (NetworkManager.Singleton.IsServer){
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy(){
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer){
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId){
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId){
        if (!NetworkManager.Singleton.IsServer)
            return;

        //Losowanie pozycji początkowej - chwilowo ustawione na 0,0,0
        Vector3 spawnPosition = new Vector3(Random.Range(0f, 0f), 0, Random.Range(0f, 0f));
        Quaternion spawnRotation = Quaternion.identity;

        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, spawnRotation);


        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        if (networkObject != null){
            networkObject.SpawnAsPlayerObject(clientId); 
        }
    }
}
