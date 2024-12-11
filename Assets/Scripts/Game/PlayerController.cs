using System;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private InputSystem _inputSystem;
    private Vector2 moveInput;

    #region Enable|Disable

    private void OnEnable()
    {
        _inputSystem = new InputSystem();
        
        _inputSystem.Player.Enable();
    }

    private void OnDisable()
    {
        _inputSystem.Player.Disable();
    }
    #endregion
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        moveInput = _inputSystem.Player.Move.ReadValue<Vector2>();

        rb.linearVelocity = moveInput * speed;
    }
}   