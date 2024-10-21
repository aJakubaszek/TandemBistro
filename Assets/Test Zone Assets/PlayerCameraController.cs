using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerCameraController : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
      public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        
        if (IsOwner){
            playerCamera.gameObject.SetActive(true);
        }
        else{
            playerCamera.gameObject.SetActive(false);
        }
    }

}
