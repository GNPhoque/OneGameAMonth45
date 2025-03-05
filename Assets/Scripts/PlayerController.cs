using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private float speedMult;
	[SerializeField] private float accelRatio;
	[SerializeField] private float rotaRatio;
	[SerializeField] private float rotaWheelie;

	private Rigidbody rb;
	private new Transform transform;

	private float inputAccel;
	private float inputBrake;
	private Vector2 inputDirection;

	#region MONOBEHAVIOUR
	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		transform = gameObject.transform;
	}

	private void FixedUpdate()
	{
		Move();
	} 
	#endregion

	#region INPUTS
	public void OnAccelerate(InputValue state)
	{
		Debug.Log(state.Get<float>());
		inputAccel = state.Get<float>();
	}

	public void OnBrake(InputValue state)
	{
		Debug.Log(state.Get<float>());
		inputBrake = state.Get<float>();
	}

	public void OnConfirm(InputValue state)
	{
		Debug.Log(state.isPressed);
	}

	public void OnCancel(InputValue state)
	{
		Debug.Log(state.isPressed);
	}

	public void OnStart(InputValue state)
	{
		Debug.Log(state.isPressed);
	}

	public void OnDirection(InputValue state)
	{
		Debug.Log(state.Get<Vector2>());
		inputDirection = state.Get<Vector2>();
	} 
	#endregion

	private void Move()
	{
		//Can move?
		//Calculate movement

		//Acceleration
		rb.AddForce(transform.forward * inputAccel * speedMult * accelRatio, ForceMode.Acceleration);

		//Steering
		Vector3 euler = rb.rotation.eulerAngles;

		float yRotation = euler.y + (inputDirection.x * Time.fixedDeltaTime * speedMult * rotaRatio);

		float maxTilt = 20f;
		float zTilt = Mathf.Clamp(inputDirection.x * rotaWheelie, -maxTilt, maxTilt);
		Quaternion newRotation = Quaternion.Euler(0f, yRotation, zTilt);
		rb.MoveRotation(newRotation);
	}
}
