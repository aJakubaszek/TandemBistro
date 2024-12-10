using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerCameraController : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    private float yRotation;
    private float xRotation;
    [SerializeField] float sensX = 1.5f;
    [SerializeField] float sensY = 1.5f;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform modelTransform;
    

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        
        if (IsOwner){
            playerCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else{
            playerCamera.gameObject.SetActive(false);
        }
    }

    public void Update(){
        if (!IsOwner) return;
        MoveCamera();
    }

    private void MoveCamera(){
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -70f, 70f);

        Quaternion targetRotation = Quaternion.Euler(xRotation, yRotation, 0);
        //cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, targetRotation, 5f*Time.deltaTime); //Fancy camera movement, may fix it someday
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        modelTransform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }

}
