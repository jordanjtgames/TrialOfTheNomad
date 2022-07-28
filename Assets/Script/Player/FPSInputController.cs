using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]

public class FPSInputController : MonoBehaviour
{
    private CharacterMotor motor;
    private Vector3 directionVector;

    Vector2 inputDirRaw;
    public static Vector2 inputDirSmoothed;
    InputAction.CallbackContext jumpInput;

    // Use this for initialization
    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        inputDirSmoothed = Vector2.Lerp(inputDirSmoothed, inputDirRaw, Time.deltaTime * 15);

        // Get the input vector from kayboard or analog stick
        if (CombatMovement.isSliding)
            directionVector = CombatMovement.slideDir;
        else
            directionVector = new Vector3(inputDirSmoothed.x, 0, inputDirSmoothed.y);
        //Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, 0);

        if (directionVector != Vector3.zero)
        {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;

            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1.0f, directionLength);

            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;

            // Multiply the normalized direction vector by the modified length
            directionVector = directionVector * directionLength;
        }

        // Apply the direction to the CharacterMotor
        motor.inputMoveDirection = transform.rotation * directionVector;
        motor.inputJump = Keyboard.current.spaceKey.ReadValue() == 1;
    }

    public void OnMovement(InputAction.CallbackContext value) { inputDirRaw = value.ReadValue<Vector2>().normalized; }
    public void OnJumpInput(InputAction.CallbackContext value) { jumpInput = value; }
}