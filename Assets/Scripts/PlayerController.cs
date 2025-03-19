using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	public PlayerUI playerUI;
	[SerializeField] private Transform graphicsTransform;

	[SerializeField] private float speedMult;
	[SerializeField] private float accelRatio;
	[SerializeField] private float rotaRatio;
	[SerializeField] private float rotaWheelie;
	[SerializeField] private float tiltMaxPerSecond;
	[SerializeField] private float cameraMaxRotationPerSecond;
	[SerializeField] private Camera camera;

	private Rigidbody rb;
	private new Transform transform;

	private float inputAccel;
	private float inputBrake;
	private float currentZTilt;
	float currentCameraForward;
	private Vector2 inputDirection;

	public event Action<PlayerController> OnRespawnPressed;
	public event Action<PlayerController> OnConfirmPressed;
	public event Action<PlayerController, Vector2> OnDirectionChanged;

	#region MONOBEHAVIOUR
	private void Awake()
	{
		GameManager.instance.OnPlayerJoined(this);

		rb = GetComponent<Rigidbody>();
		transform = gameObject.transform;

		rb.centerOfMass = Vector3.zero;
		currentCameraForward = camera.transform.rotation.eulerAngles.y;
	}

	private void FixedUpdate()
	{
		Move();
	} 
	#endregion

	#region INPUTS
	public void OnAccelerate(InputValue state)
	{
		//Debug.Log(state.Get<float>());
		inputAccel = state.Get<float>();
	}

	public void OnBrake(InputValue state)
	{
		//Debug.Log(state.Get<float>());
		inputBrake = state.Get<float>();
	}

	public void OnConfirm(InputValue state)
	{
		Debug.Log(state.isPressed);
		OnConfirmPressed?.Invoke(this);
	}

	public void OnCancel(InputValue state)
	{
		Debug.Log(state.isPressed);
		OnRespawnPressed?.Invoke(this);
	}

	public void OnStart(InputValue state)
	{
		Debug.Log(state.isPressed);
		MinigameManager.instance.StartManager();
	}

	public void OnDirection(InputValue state)
	{
		//Debug.Log(state.Get<Vector2>());
		inputDirection = state.Get<Vector2>();
		OnDirectionChanged?.Invoke(this, inputDirection);
	} 
	#endregion

	private void Move()
	{
		//Can move?

		//Acceleration
		Vector3 accel = transform.forward* inputAccel * speedMult * accelRatio;
		rb.AddForce(accel, ForceMode.Force);
		Debug.DrawLine(transform.position + rb.centerOfMass, transform.position + rb.centerOfMass + accel, Color.red);

		//Steering
		Vector3 euler = rb.rotation.eulerAngles;
		float yRotation = euler.y + (inputDirection.x * Time.fixedDeltaTime * speedMult * rotaRatio);

		//Tilt
		currentZTilt = Mathf.MoveTowards(currentZTilt, rotaWheelie * inputDirection.x, tiltMaxPerSecond * Time.fixedDeltaTime);

		float zTilt = Mathf.Clamp(
			currentZTilt,
			-rotaWheelie, 
			rotaWheelie
		);
		graphicsTransform.rotation = Quaternion.Euler(euler.x, yRotation, euler.z + zTilt);

		//Camera smoothing
		Vector3 cameraEuler = camera.transform.rotation.eulerAngles;
		if(currentCameraForward - euler.y > 180)
		{
			currentCameraForward -= 360;
		}
		else if (euler.y - currentCameraForward > 180)
		{
			currentCameraForward += 360;
		}
		currentCameraForward = Mathf.MoveTowards(currentCameraForward, euler.y, cameraMaxRotationPerSecond * Time.fixedDeltaTime);
		camera.transform.rotation = Quaternion.Euler(22f, currentCameraForward, 0f);

		Quaternion newRotation = Quaternion.Euler(euler.x, yRotation, euler.z);
		rb.MoveRotation(newRotation);
	}

	public void ChangeAccel(float change)
	{
		accelRatio += change;
	}
}
