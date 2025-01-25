using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;

public class StartGame : NetworkBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject vrPrefab;
    [SerializeField] TextMeshProUGUI PcorVR;

    [SerializeField] private Dictionary<ulong, string> playerTypes = new Dictionary<ulong, string>();

    void Start() {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;

        if (startButton != null) {
            startButton.onClick.AddListener(StartGameSrv);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void HandleClientConnected(ulong clientId) {
        if (clientId == NetworkManager.Singleton.LocalClientId) {
            string playerType = DetectPlayerType();
            RegisterPlayerTypeServerRpc(playerType);
        }
    }

    private void StartGameSrv() {
        if (!IsServer) return;

        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneChanged;
        NetworkManager.Singleton.SceneManager.LoadScene("AmericanBistro", LoadSceneMode.Single);
    }

    void OnDisable() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneChanged;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RegisterPlayerTypeServerRpc(string type, ServerRpcParams rpcParams = default) {
        ulong clientId = rpcParams.Receive.SenderClientId;
        if (!playerTypes.ContainsKey(clientId)) {
            playerTypes.Add(clientId, type);
            Debug.Log(type);
            PcorVR.text = type;
        }
    }

    void OnSceneChanged(SceneEvent sceneEvent) {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted) {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsIds) {
                SpawnPlayer(client);
            }
        }
    }

    private void SpawnPlayer(ulong clientId) {
        if (!NetworkManager.Singleton.IsServer) return;
        Vector3 spawnPosition = GameObject.Find("PCTransform").transform.position;
        Quaternion spawnRotation = Quaternion.identity;

        GameObject prefabToSpawn = playerTypes.ContainsKey(clientId) && playerTypes[clientId] == "VR" ? vrPrefab : playerPrefab;

        GameObject playerInstance = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
        var networkObject = playerInstance.GetComponent<NetworkObject>();
        if (networkObject != null) {
            networkObject.SpawnAsPlayerObject(clientId);
        }
        else{
            Debug.LogError("Network object not found");
        }
    }

    private string DetectPlayerType() {
        bool isVR = isPresent();
        return isVR ? "VR" : "PC";
    }

    public static bool isPresent(){
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems){
            if (xrDisplay.running){
                Debug.Log("has vr");
                return true;
            }
        }
        Debug.Log("Does not have vr");
        return false;
    }
}
