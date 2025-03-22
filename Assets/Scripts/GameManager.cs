using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] Transform startPosition;
	[SerializeField] private List<Checkpoint> checkpoints;
	[SerializeField] public List<PlayerController> playerControllers;
	[SerializeField] public List<PlayerController> orderedPlayersList;
	[SerializeField] public List<Checkpoint> distanceCheckpoints;

	[SerializeField] public float lapDistance;
	[SerializeField] public int raceLaps;
	[SerializeField] public float positionRefreshDelay;

	[SerializeField] private LayerMask[] playerCamMasks;
	[SerializeField] private LayerMask[] playerVCamMasks;

	private Dictionary<PlayerController, Checkpoint> playerCheckpoints;
	private Dictionary<PlayerController, Checkpoint> playerDistanceCheckpoints;

	public event Action OnStartRacePressed;

	public static GameManager instance;

	public Action<InputValue> PlayerController_OnStartRacePressed { get; private set; }

	private void Awake()
	{
		if(instance != null)
		{
			return;
		}

		instance = this;

		playerCheckpoints = new Dictionary<PlayerController, Checkpoint>();
		playerDistanceCheckpoints = new Dictionary<PlayerController, Checkpoint>();

		StartCoroutine(GetPlayersPositions());
	}

	private IEnumerator GetPlayersPositions()
	{
		while (true)
		{
			yield return new WaitForSeconds(positionRefreshDelay);

			for (int i = 0; i < playerControllers.Count; i++)
			{
				PlayerController pc = playerControllers[i];
				pc.distance = lapDistance * pc.currentLap;

				if (playerDistanceCheckpoints[pc] != distanceCheckpoints.Last())
				{
					for (int j = 0; j <= distanceCheckpoints.IndexOf(playerDistanceCheckpoints[pc]); j++)
					{
						pc.distance += distanceCheckpoints[j].distance;
					}
				}

				pc.distance += Vector3.Distance(playerDistanceCheckpoints[pc].transform.position, pc.transform.position);
			}
			orderedPlayersList = orderedPlayersList.OrderByDescending(x => x.distance).ToList();
			for (int i = 0; i < orderedPlayersList.Count; i++)
			{
				orderedPlayersList[i].UpdatePosition(i);
			}
		}
	}

	public void OnPlayerJoined(PlayerController pc)
	{
		playerControllers.Add(pc);

		pc.currentLap = 1;

		orderedPlayersList.Add(pc);
		playerCheckpoints.Add(pc, checkpoints.Last());
		foreach (var checkpoint in checkpoints)
		{
			checkpoint.OnPlayerEntered += CheckCheckPoint;
		}
		playerDistanceCheckpoints.Add(pc, distanceCheckpoints.Last());
		foreach (var checkpoint in distanceCheckpoints)
		{
			checkpoint.OnPlayerEntered += CheckCheckPoint;
		}
		pc.OnRespawnPressed += PlayerController_OnRespawnPressed;
		pc.OnConfirmPressed += Pc_OnConfirmPressed;

		pc.transform.position = startPosition.position;

		pc.camera.cullingMask = playerCamMasks[playerControllers.Count - 1];
		pc.vCamera.gameObject.layer = LayerMask.NameToLayer($"P{playerControllers.Count}Cam");
	}

	private void Pc_OnConfirmPressed(PlayerController obj)
	{
		foreach (var item in playerControllers)
		{
			item.OnConfirmPressed -= Pc_OnConfirmPressed;
		}

		OnStartRacePressed?.Invoke();
	}

	public void PlayerController_OnRespawnPressed(PlayerController pc)
	{
		pc.transform.position = playerCheckpoints[pc].transform.position;
		pc.transform.rotation = playerCheckpoints[pc].transform.rotation;
		pc.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	private void CheckCheckPoint(PlayerController pc, Checkpoint checkpoint)
	{
		if (checkpoints.Contains(checkpoint))
		{
			CheckLapCheckpoint(pc, checkpoint);
		}
		else if (distanceCheckpoints.Contains(checkpoint))
		{
			CheckDistanceCheckpoint(pc, checkpoint);
		}
	}

	private void CheckLapCheckpoint(PlayerController pc, Checkpoint checkpoint)
	{
		print($"{pc.name} passed a lap checkpoint {checkpoint.name}");

		Checkpoint previous = playerCheckpoints[pc];

		//Only valid checkpoint is the first one
		if (previous == checkpoints.Last())
		{
			if (checkpoint != checkpoints[0])
			{
				print($"{pc.name} went through the wrong checkpoint");
				return;
			}
			playerCheckpoints[pc] = checkpoint;
		}
		else
		{
			if (checkpoint != checkpoints[checkpoints.IndexOf(previous) + 1])
			{
				print($"{pc.name} went through the wrong checkpoint");
				return;
			}
			playerCheckpoints[pc] = checkpoint;
		}

		if (checkpoint == checkpoints.Last())
		{
			print($"{pc.name} finished a lap");
			pc.LapCompleted();
		}
	}

	private void CheckDistanceCheckpoint(PlayerController pc, Checkpoint checkpoint)
	{
		print($"{pc.name} passed a distance checkpoint {checkpoint.name}");

		Checkpoint previous = playerDistanceCheckpoints[pc];

		//Only valid checkpoint is the first one
		if (previous == distanceCheckpoints.Last())
		{
			if (checkpoint != distanceCheckpoints[0])
			{
				print($"{pc.name} went through the wrong distance checkpoint");
				return;
			}
			playerDistanceCheckpoints[pc] = checkpoint;
		}
		else
		{
			if (checkpoint != distanceCheckpoints[distanceCheckpoints.IndexOf(previous) + 1])
			{
				print($"{pc.name} went through the wrong distance checkpoint");
				return;
			}
			playerDistanceCheckpoints[pc] = checkpoint;
		}
	}
}
