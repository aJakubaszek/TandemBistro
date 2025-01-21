using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class UnityRelay : MonoBehaviour
{
    [SerializeField] Button hostButton; //Zrobić jakieś DontDestroyOnLoad i ręcznie zarządzać spawnowaniem
    [SerializeField] Button joinButton;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI codeOutput;
    private async void Start(){
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostButton.onClick.AddListener(()=>{
            CreateRelay();
        });
        joinButton.onClick.AddListener(()=>{
            JoinRelay();
        });
    }

    private async void CreateRelay(){
        try{
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log(joinCode);
        codeOutput.text = "Joining Relay with " + joinCode;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        NetworkManager.Singleton.StartHost();
        }
        catch(RelayServiceException e){
            Debug.Log(e);
        }
    }

    private object UnityTransport()
    {
        throw new NotImplementedException();
    }

    private async void JoinRelay(){
        try{
            string joinCode = inputField.text;
            Debug.Log("Joining Relay with "+ joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
        } catch (RelayServiceException e){
            Debug.Log(e);
        }
    }
}
