using UnityEngine;

public class CameraController : MonoBehaviour
{
	/// <summary>
	/// Speed of the camera movement
	/// </summary>
	private float Speed = 1;

	/// <summary>
	/// Updates the position and rotation of the camera
	/// </summary>
	private void Update()
	{
		// Change the movement speed with scroll wheel
		Speed += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 20;

		// Move the camera acording to rotation and input
		transform.position += transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * Speed;

		// Rotate the camera using mouse
		transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * Time.deltaTime * 50;
	}
}