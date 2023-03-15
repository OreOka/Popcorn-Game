using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{

    public static GameInput Instance { get; private set; }



    public event EventHandler OnDodgePopAction_completed;
    public event EventHandler OnDodgePopAction_started;
    public event EventHandler OnPopAction_completed;
    public event EventHandler OnPopAction_started;
    public event EventHandler onMovePerformed;
    public event EventHandler onMoveStarted;

    //private const string PLAYER_INPUT_BINDINGS

    Dandy playerInputActions;

    private void Awake()
    {
        Instance = this;
        playerInputActions = new Dandy();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Pop.performed += Pop_completed;
        //playerInputActions.Player.Pop.canceled += Pop_completed;

        playerInputActions.Player.Dodge.performed += Dodge_completed;
     //   playerInputActions.Player.Dodge.canceled += Dodge_completed;

        playerInputActions.Player.Pop.started += Pop_started;
        playerInputActions.Player.Dodge.started += Dodge_started; ;

        playerInputActions.Player.Move.performed += Move_performed;
        playerInputActions.Player.Move.started += Move_started;


    }

    
    private void OnDestroy()
    {
        playerInputActions.Player.Pop.performed -= Pop_completed;
        playerInputActions.Player.Dodge.performed -= Dodge_completed;

      //  playerInputActions.Player.Pop.canceled -= Pop_completed;
       // playerInputActions.Player.Dodge.canceled -= Dodge_completed;

        playerInputActions.Player.Dodge.started -= Dodge_started;
        playerInputActions.Player.Pop.started -= Pop_started;

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

    

    private void Dodge_completed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       OnDodgePopAction_completed?.Invoke(this, EventArgs.Empty);
       

    }

    private void Pop_completed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       OnPopAction_completed?.Invoke(this, EventArgs.Empty);
       
    }
    private void Dodge_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        print("DODGE start");

    }

    private void Pop_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        print("POP start");

    }


    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector;
    }

}
