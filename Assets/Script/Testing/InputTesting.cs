using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTesting : MonoBehaviour
{
    Vector3 inputMovement;
    bool crouched = false;

    InputAction jumpAction;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(inputMovement);

        transform.localScale = crouched ? Vector3.one * 0.5f : Vector3.one;

        if (PlayerInput.isJumpKeyHeld) {
            
        }
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 rawInputMovement = value.ReadValue<Vector2>();
        inputMovement = new Vector3(rawInputMovement.x, 0, rawInputMovement.y);
    }

    public void OnJump(InputAction.CallbackContext value) {
        transform.position += Vector3.up;
    }

    public void OnCrouch(InputAction.CallbackContext value)
    {
        

        jumpAction = value.action;

        crouched = value.performed;
    }

    public bool OnBoolCrouch(InputAction.CallbackContext value)
    {
        return true;
    }
}
