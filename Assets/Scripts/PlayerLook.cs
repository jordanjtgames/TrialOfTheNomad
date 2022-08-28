#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerLook : MonoBehaviour
{
    const float k_MouseSensitivityMultiplier = 0.01f;

    public float mouseSensitivity = 100f;

    public Transform playerBody;
    public Transform wallrunLook;

    float xRotation = 0f;

    public bool isWallrunning = false;

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
#if ENABLE_INPUT_SYSTEM
        float mouseX = 0, mouseY = 0;

        if (Mouse.current != null) {
            var delta = Mouse.current.delta.ReadValue() / 15.0f;
            mouseX += delta.x;
            mouseY += delta.y;
        }
        if (Gamepad.current != null) {
            //Vector2 value = Gamepad.current.rightStick.ReadValue() * 2;
            Vector2 value = Inputs.look;
            float deadzone = 0.15f;
            if(Mathf.Abs(Gamepad.current.rightStick.ReadValue().x) > deadzone || Mathf.Abs(Gamepad.current.rightStick.ReadValue().y) > deadzone) { 
            }
            else
                value = Vector2.zero;
            mouseX += value.x;
            mouseY += value.y;
        }

        mouseX *= mouseSensitivity * k_MouseSensitivityMultiplier * (UIManager.weaponWheelOpen ? 0 : 1);
        mouseY *= mouseSensitivity * k_MouseSensitivityMultiplier * (UIManager.weaponWheelOpen ? 0 : 1);
#else
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * k_MouseSensitivityMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * k_MouseSensitivityMultiplier;
#endif

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (!isWallrunning) {
            playerBody.Rotate(Vector3.up * mouseX);
        } else {
            wallrunLook.Rotate(Vector3.up * mouseX);
        }
        
    }
}
