using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] Transform startPosition;
	[SerializeField] private List<Checkpoint> checkpoints;
	[SerializeField] private List<PlayerUI> playerUIs;
	[SerializeField] public List<PlayerController> playerControllers;

	private Dictionary<PlayerController, Checkpoint> playerCheckpoints;

	public static GameManager instance;

	private void Awake()
	{
		if(instance != null)
		{
			return;
		}

		instance = this;

		playerCheckpoints = new Dictionary<PlayerController, Checkpoint>();
	}

	public void OnPlayerJoined(PlayerController pc)
	{
		playerControllers.Add(pc);

		pc.playerUI = playerUIs[playerControllers.Count - 1];

		playerCheckpoints.Add(pc, checkpoints.Last());
		foreach (var checkpoint in checkpoints)
		{
			checkpoint.OnPlayerEntered += CheckCheckPoint;
		}
		pc.OnRespawnPressed += PlayerController_OnRespawnPressed;

		pc.transform.position = startPosition.position;
	}

	public void PlayerController_OnRespawnPressed(PlayerController pc)
	{
		pc.transform.position = playerCheckpoints[pc].transform.position;
		pc.transform.rotation = playerCheckpoints[pc].transform.rotation;
		pc.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	private void CheckCheckPoint(PlayerController pc, Checkpoint checkpoint)
	{
		print($"{pc.name} passed a checkpoint {checkpoint.name}");

		Checkpoint previous = playerCheckpoints[pc];

		//Only valid checkpoint is the first one
		if(previous == checkpoints.Last())
		{
			if(checkpoint != checkpoints[0])
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

		if(checkpoint == checkpoints.Last())
		{
			print($"{pc.name} finished a lap");
		}
	}
}
