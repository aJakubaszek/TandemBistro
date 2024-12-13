using TMPro;
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
        }
        catch(RelayServiceException e){
            Debug.Log(e);
        }
    }

    private async void JoinRelay(){
        try{
            string joinCode = inputField.text;
            Debug.Log("Joining Relay with "+ joinCode);
            await RelayService.Instance.JoinAllocationAsync(joinCode);
        } catch (RelayServiceException e){
            Debug.Log(e);
        }
    }
}
