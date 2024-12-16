using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
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

        Vector3 spawnPosition = new Vector3(7f, 0, 0f);
        Quaternion spawnRotation = Quaternion.identity;

        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, spawnRotation);


        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        if (networkObject != null){
            networkObject.SpawnAsPlayerObject(clientId); 
            playerInstance.transform.position = spawnPosition;
            Debug.Log("Player spawned");
        }
        else{
            Debug.Log("Network object not found");
        }
    }
}
