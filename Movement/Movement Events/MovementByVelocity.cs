using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Required Components
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
#endregion

public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D theRb;
    private MovementByVelocityEvent movementByVelEvent;

    private void Awake()
    {
        theRb = GetComponent<Rigidbody2D>();
        movementByVelEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        //Suscribe to the movement by velocity event
        movementByVelEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        //Unsuscribe to the movement by velocity event
        movementByVelEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    /// <summary>
    /// Movement By Velocity Event Handler
    /// </summary>
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        MoveRigidBody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
    }

    /// <summary>
    /// Moves The Rigid Body
    /// </summary>
    private void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
    {
        theRb.velocity = moveDirection * moveSpeed;
    }
}
