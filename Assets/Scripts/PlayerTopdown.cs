using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
public class PlayerTopdown : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 6;
    private Controller2D _controller;
    private Vector2 _directionalInput;
    private Vector2 _velocity;
    
    private float _velocityXSmoothing;
    private float _velocityYSmoothing;
    [SerializeField] private float _accelerationTime = .1f;

    private void Awake()
    {
        _controller = GetComponent<Controller2D> ();
    }
    
    public void SetDirectionalInput (InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();
        _directionalInput = input;
    }

    public void HandleAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Dash();
        }
    }

    private void Dash()
    {
        Debug.Log("Example Action");
    }

    private void FixedUpdate()
    {
        CalculateVelocity ();
        
        _controller.Move(_velocity * Time.fixedDeltaTime);
    }
    
    // private void Update()
    // {
    //     CalculateVelocity ();
    //     
    //     _controller.Move(_velocity * Time.deltaTime);
    // }
    
    private void CalculateVelocity() {
        var targetVelocityX = _directionalInput.x * _moveSpeed;
        var targetVelocityY = _directionalInput.y * _moveSpeed;
        _velocity.x = Mathf.SmoothDamp (
            _velocity.x, 
            targetVelocityX, 
            ref _velocityXSmoothing, 
            _accelerationTime
        );
        _velocity.y = Mathf.SmoothDamp (
            _velocity.y, 
            targetVelocityY, 
            ref _velocityYSmoothing, 
            _accelerationTime
        );
    }
}
