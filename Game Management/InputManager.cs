using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    private PlayerInput inputActions;



    protected override void Awake()
    {
        base.Awake();

        inputActions = new();
    }
}
