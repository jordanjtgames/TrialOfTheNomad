using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static Vector2 inputDirRaw = Vector2.zero;
    public static Vector2 lookDirRaw = Vector2.zero;

    public static bool isJumpKeyHeld = false;
    public static bool isJumpKeyPressed = false;
    bool jumpKeyOneFrame = false;

    public static bool isSlideKeyHeld = false;
    public static bool isSlideKeyPressed = false;

    public static bool isCrouchKeyHeld = false;
    public static bool isSprintKeyHeld = false;

    public InputAction.CallbackContext d;

    void Awake()
    {

    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {

    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 input = value.ReadValue<Vector2>();
        inputDirRaw = new Vector2(input.x, input.y);
    }

    public void OnJump(InputAction.CallbackContext value)
    {

        if (value.started) {
            isJumpKeyHeld = true;
            Debug.Log("STARTED");
        }

        
        if (value.canceled) {
            isJumpKeyPressed = false;
            isJumpKeyHeld = false;
        }
    }

    public void OnSlide(InputAction.CallbackContext value)
    {

    }

    public void OnCrouch(InputAction.CallbackContext value)
    {

    }

    public void OnSprint(InputAction.CallbackContext value)
    {

    }

    public void OnLook(InputAction.CallbackContext value)
    {
        Vector2 input = value.ReadValue<Vector2>();
        lookDirRaw = new Vector2(input.x, input.y);
    }



    void Update()
    {
        
    }
}
