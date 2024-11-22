using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float horizontalInput;
    [SerializeField] float verticalInput;
    [SerializeField] Transform bodyTransform;
    [SerializeField] Rigidbody rb;

    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        if (!IsOwner) return;
        GetInput();
    }

     private void FixedUpdate() {
        if (!IsOwner) return;
        Move();
    }

    private void GetInput(){
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }
    private void Move(){
    Vector3 moveDirection = (bodyTransform.forward * verticalInput + bodyTransform.right * horizontalInput).normalized;
    if (moveDirection.magnitude > 0){
        rb.velocity = moveDirection * moveSpeed;
    }
    else{
        rb.velocity = Vector3.zero;  // Zatrzymanie gracza
    }
    }
}
