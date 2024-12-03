using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkClientManager : MonoBehaviour{
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] TMP_InputField inputField;
    private void Awake(){
        hostButton.onClick.AddListener(()=>{
            Debug.Log("woke up");
            NetworkManager.Singleton.StartHost();
        });
        joinButton.onClick.AddListener(()=>{
            JoinGame();
        });
    }

    private void JoinGame(){
        Debug.Log("aaaaa");
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = inputField.text;
        NetworkManager.Singleton.StartClient();
    }
}
