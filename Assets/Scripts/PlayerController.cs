using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private Transform graphicsTransform;

	[SerializeField] private float speedMult;
	[SerializeField] private float accelRatio;
	[SerializeField] private float rotaRatio;
	[SerializeField] private float rotaWheelie;

	private Rigidbody rb;
	private new Transform transform;

	private float inputAccel;
	private float inputBrake;
	private Vector2 inputDirection;

	public event Action<PlayerController> OnRespawnPressed;
	public event Action<PlayerController> OnConfirmPressed;
	public event Action<Vector2> OnDirectionChanged;

	#region MONOBEHAVIOUR
	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		transform = gameObject.transform;

		rb.centerOfMass = Vector3.zero;
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
	}

	public void OnDirection(InputValue state)
	{
		Debug.Log(state.Get<Vector2>());
		inputDirection = state.Get<Vector2>();
		OnDirectionChanged?.Invoke(inputDirection);
	} 
	#endregion

	private void Move()
	{
		//Can move?

		//Acceleration
		rb.AddForce(transform.forward * inputAccel * speedMult * accelRatio, ForceMode.Force);
		Debug.DrawLine(transform.position + rb.centerOfMass, transform.position + rb.centerOfMass + transform.forward * inputAccel * speedMult * accelRatio, Color.red);

		//Steering
		Vector3 euler = rb.rotation.eulerAngles;
		//print(inputDirection.x * Time.fixedDeltaTime * speedMult * rotaRatio);
		float yRotation = euler.y + (inputDirection.x * Time.fixedDeltaTime * speedMult * rotaRatio);

		//Tilt
		float zTilt = Mathf.Clamp(inputDirection.x * rotaWheelie, -rotaWheelie, rotaWheelie);
		graphicsTransform.rotation = Quaternion.Euler(euler.x, yRotation, euler.z + zTilt);

		Quaternion newRotation = Quaternion.Euler(euler.x, yRotation, euler.z);
		rb.MoveRotation(newRotation);
	}
}
