using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
	[SerializeField] public Camera camera;
	[SerializeField] public CinemachineVirtualCamera vCamera;

	[SerializeField] TextMeshProUGUI lapCounterText;
	[SerializeField] TextMeshProUGUI positionText;

	private Rigidbody rb;
	private new Transform transform;

	private bool canMove = true;
	public int currentLap;
	private float inputAccel;
	public float distance;
	private float inputBrake;
	private float currentZTilt;
	private float currentCameraForward;
	private Vector2 inputDirection;

	public event Action<PlayerController> OnRespawnPressed;
	public event Action<PlayerController> OnConfirmPressed;
	public event Action<PlayerController> OnInputPressed;
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

	private void Update()
	{
		if (!Mathf.Approximately(inputAccel, 0f) || !Mathf.Approximately(inputDirection.x, 0f))
		{
			OnInputPressed?.Invoke(this);
		}
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
		if(!Mathf.Approximately(inputAccel, 0f))
		{
			OnInputPressed?.Invoke(this);
		}
	}

	public void OnBrake(InputValue state)
	{
		//Debug.Log(state.Get<float>());
		inputBrake = state.Get<float>();
		if (!Mathf.Approximately(inputBrake, 0f))
		{
			OnInputPressed?.Invoke(this);
		}
	}

	public void OnConfirm(InputValue state)
	{
		Debug.Log(state.isPressed);
		OnConfirmPressed?.Invoke(this);
		OnInputPressed?.Invoke(this);
	}

	public void OnCancel(InputValue state)
	{
		Debug.Log(state.isPressed);
		OnRespawnPressed?.Invoke(this);
		OnInputPressed?.Invoke(this);
	}

	public void OnStart(InputValue state)
	{
		Debug.Log(state.isPressed);
	}

	public void OnDirection(InputValue state)
	{
		//Debug.Log(state.Get<Vector2>());
		inputDirection = state.Get<Vector2>();
		OnDirectionChanged?.Invoke(this, inputDirection);
		if (!Mathf.Approximately(inputDirection.magnitude, 0f))
		{
			OnInputPressed?.Invoke(this);
		}
	} 
	#endregion

	private void Move()
	{
		//Can move?
		if (!canMove)
		{
			return;
		}

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

		Quaternion newRotation = Quaternion.Euler(euler.x, yRotation, euler.z);
		rb.MoveRotation(newRotation);
	}

	public void ChangeAccel(float change)
	{
		accelRatio += change;
	}

	public void LapCompleted()
	{
		currentLap++;
		lapCounterText.text = $"Lap : {currentLap}/{GameManager.instance.raceLaps}";
		if(currentLap == GameManager.instance.raceLaps)
		{
			//TODO : qqch
		}
	}

	public void UpdatePosition(int position)
	{
		positionText.text = $"Pos : {position + 1}";
	}

	public void DisableMovement()
	{
		canMove = false;
	}

	public void EnableMovement()
	{
		canMove = true;
	}

	public IEnumerator SetFOV(float fov)
	{
		float fovBefore = vCamera.m_Lens.FieldOfView;
		float elapsed = 0;
		while (elapsed < .2f)
		{
			vCamera.m_Lens.FieldOfView = Mathf.Lerp(fovBefore, fov, elapsed / 0.2f);
			yield return null;
			elapsed += Time.deltaTime;
		}
	}
}
