using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{

    public static GameInput Instance { get; private set; }



    public event EventHandler OnDodgePopAction;
    public event EventHandler OnPopAction;
    public event EventHandler onMovePerformed;
    public event EventHandler onMoveStarted;

    //private const string PLAYER_INPUT_BINDINGS

    Dandy playerInputActions;

    private void Awake()
    {
        Instance = this;
        playerInputActions = new Dandy();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Pop.performed += Pop_performed;
        playerInputActions.Player.Dodge.performed += Dodge_performed;

        playerInputActions.Player.Move.performed += Move_performed;
        playerInputActions.Player.Move.started += Move_started;


    }
    private void OnDestroy()
    {
        playerInputActions.Player.Pop.performed -= Pop_performed;
        playerInputActions.Player.Dodge.performed -= Dodge_performed;

        playerInputActions.Player.Move.performed -= Move_performed;
        playerInputActions.Player.Move.started -= Move_started;

    }

    private void Move_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        onMoveStarted?.Invoke(this, EventArgs.Empty);
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        onMovePerformed?.Invoke(this, EventArgs.Empty);
    }

    

    private void Dodge_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       OnDodgePopAction?.Invoke(this, EventArgs.Empty);
    }

    private void Pop_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       OnPopAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector;
    }

}
