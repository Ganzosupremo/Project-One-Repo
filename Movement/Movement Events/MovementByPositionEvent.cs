using System;
using UnityEngine;

[DisallowMultipleComponent]
public class MovementByPositionEvent : MonoBehaviour
{
    public event Action<MovementByPositionEvent, MovementByPositionArgs> OnMovementToPosition;

    public void CallMovementByPositionEvent(Vector3 movePos, Vector3 currentPos, float moveSpeed, Vector2 moveDirection, bool isRolling = false)
    {
        OnMovementToPosition?.Invoke(this, new MovementByPositionArgs()
        {
            movePosition = movePos,
            currentPosition = currentPos,
            moveSpeed = moveSpeed,
            moveDirection = moveDirection,
            isRolling = isRolling
        });
    }
}


public class MovementByPositionArgs : EventArgs
{
    public Vector3 movePosition;
    public Vector3 currentPosition;
    public float moveSpeed;
    public Vector2 moveDirection;
    public bool isRolling;
}