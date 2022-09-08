using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D theRb;
    private IdleEvent theIdleEvent;

    private void Awake()
    {
        theRb = GetComponent<Rigidbody2D>();
        theIdleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        //Suscribe to the idle event
        theIdleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        //Unsuscribe to the idle event
        theIdleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();
    }

    /// <summary>
    /// Moves The Rigid Body
    /// </summary>
    private void MoveRigidBody()
    {
        theRb.velocity = Vector2.zero;
    }
}
