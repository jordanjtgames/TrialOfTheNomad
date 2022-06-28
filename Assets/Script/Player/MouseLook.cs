using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	public float rotationY = 0F;

	Vector2 lookAxis;

	float minimumY_Default = -85;
	void Update ()
	{
		//Time.timeScale = 0.5f;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + lookAxis.x * sensitivityX;
			
			rotationY += lookAxis.y * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, lookAxis.x * sensitivityX, 0);
		}
		else
		{
			rotationY += lookAxis.y * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}

		if (CombatMovement.isSliding || CombatMovement.currentSlideDelay > 0)
			minimumY = Mathf.Lerp(minimumY, -20f, Time.deltaTime * 12);
		else
			minimumY = Mathf.Lerp(minimumY, minimumY_Default, Time.deltaTime * 12);
	}

	void Start ()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;

		minimumY_Default = minimumY;
	}

	public void OnLook(InputAction.CallbackContext value)
    {
		if (value.action.activeControl.layout.ToString() == "Button")
			lookAxis = value.ReadValue<Vector2>() * 0.5f;
		else
			lookAxis = value.ReadValue<Vector2>() * 0.025f;
		//Debug.Log();
	}
}