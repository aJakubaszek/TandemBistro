using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : NetworkBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] GameObject playerPrefab;
    void Start(){
       startButton.onClick.AddListener(StartGameSrv);
       DontDestroyOnLoad(gameObject);
    }

    private void StartGameSrv(){
        if(!IsServer){return;}
        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneChanged;
        NetworkManager.Singleton.SceneManager.LoadScene("AmericanBistro", LoadSceneMode.Single);
    }

    void OnEnable() {
        
    }

    void OnDisable() {
        NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneChanged;
    }

    void OnSceneChanged(SceneEvent sceneEvent) {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted) {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsIds){
                SpawnPlayer(client);
            }
        }
    }

     void SpawnPlayer(ulong clientId) {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Vector3 spawnPosition = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        Quaternion spawnRotation = Quaternion.identity;

        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, spawnRotation);

        var networkObject = playerInstance.GetComponent<NetworkObject>();
        if (networkObject != null){
            networkObject.SpawnAsPlayerObject(clientId);
        }
    }
}
