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
		Quaternion quaternion = Quaternion.Euler(0f, inputDirection.x * Time.fixedDeltaTime * speedMult * rotaRatio, 0f);
		rb.MoveRotation(rb.rotation * quaternion);
	}
}
