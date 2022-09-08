using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByPositionEvent))]
[DisallowMultipleComponent]
public class MovementByPosition : MonoBehaviour
{
    private Rigidbody2D theRb;
    private MovementByPositionEvent movementByPositionEvent;

    private void Awake()
    {
        theRb = GetComponent<Rigidbody2D>();
        movementByPositionEvent = GetComponent<MovementByPositionEvent>();
    }

    private void OnEnable()
    {
        //Suscribe to the movement by velocity event
        movementByPositionEvent.OnMovementToPosition += MovementByPositionEvent_OnMovementByPosition;
    }

    private void OnDisable()
    {
        //Unsuscribe to the movement by velocity event
        movementByPositionEvent.OnMovementToPosition -= MovementByPositionEvent_OnMovementByPosition;
    }

    /// <summary>
    /// The On Movement Event
    /// </summary>
    private void MovementByPositionEvent_OnMovementByPosition(MovementByPositionEvent movementByPositionEvent, MovementByPositionArgs movementByPositionArgs)
    {
        MoveRigidBody(movementByPositionArgs.movePosition, movementByPositionArgs.currentPosition, movementByPositionArgs.moveSpeed);
    }

    /// <summary>
    /// Moves The Rigid Body
    /// </summary>
    private void MoveRigidBody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
    {
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);
        theRb.MovePosition(theRb.position + (moveSpeed * Time.fixedDeltaTime * unitVector));
    }
}
