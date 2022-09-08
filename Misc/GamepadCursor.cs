using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class GamepadCursor : MonoBehaviour
{
    [SerializeField] private UnityEngine.InputSystem.PlayerInput currentPlayerInput;
    [SerializeField] private RectTransform cursorRectTransform;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private float cursorSpeed = 1500f;

    private Mouse virtualMouse;
    private Mouse currentMouse;
    private bool previousMouseState;
    private const string gamepadScheme = "Gamepad";
    private const string keyboardScheme = "Keyboard&Mouse";
    private string previousControlScheme = "";

    private void OnEnable()
    {
        currentMouse = Mouse.current;
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, currentPlayerInput.user);

        if (cursorRectTransform != null)
        {
            Vector2 position = cursorRectTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMousePosition;
        currentPlayerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        InputSystem.onAfterUpdate -= UpdateMousePosition;
        currentPlayerInput.onControlsChanged -= OnControlsChanged;
        InputSystem.RemoveDevice(virtualMouse);
    }

    public void OnControlsChanged(UnityEngine.InputSystem.PlayerInput playerInput)
    {
        if (playerInput.currentControlScheme == keyboardScheme && previousControlScheme != keyboardScheme)
        {
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlScheme = keyboardScheme;
        }
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            AnchorCursor(currentMouse.position.ReadValue());
            previousControlScheme = gamepadScheme;
        }
    }

    /// <summary>
    /// Updates the motion of the virtual mouse
    /// </summary>
    private void UpdateMousePosition()
    {
        if (virtualMouse != null && Gamepad.current != null)
        {
            Vector2 deltaValue = Gamepad.current.rightStick.ReadValue();
            deltaValue *= cursorSpeed * Time.deltaTime;

            Vector2 cursorPosition = virtualMouse.position.ReadValue();
            Vector2 newPosition = cursorPosition + deltaValue;

            //Uncomment if I want to add some clamp to the virtual mouse
            //newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
            //newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);

            InputState.Change(virtualMouse.position, newPosition);
            InputState.Change(virtualMouse.delta, deltaValue);

            //bool aButtonIsPressed = Gamepad.current.aButton.IsPressed();
            if (previousMouseState != Gamepad.current.aButton.isPressed)
            {
                virtualMouse.CopyState<MouseState>(out var mouseState);
                mouseState.WithButton(MouseButton.Left, Gamepad.current.aButton.IsPressed());
                InputState.Change(virtualMouse, mouseState);
                previousMouseState = Gamepad.current.aButton.isPressed;
            }

            AnchorCursor(newPosition);
        }
    }

    /// <summary>
    /// Move the position of the cursor
    /// </summary>
    /// <param name="position">The position we want to move the cursor to</param>
    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, null, out anchoredPosition);

        cursorRectTransform.anchoredPosition = anchoredPosition;
    }
}
