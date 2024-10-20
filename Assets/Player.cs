using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;
    private Rigidbody rb;
    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        if (!IsOwner) return;
        Move();
    }

    private void Move(){
        float moveDirection = Input.GetAxis("Vertical");
        float rotationDirection = Input.GetAxis("Horizontal");

        Vector3 move = transform.forward * moveDirection * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + move);

        float rotation = rotationDirection * rotationSpeed * Time.deltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));
    }
}
