using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private List<Checkpoint> checkpoints;
	[SerializeField] private PlayerController playerController;

	private Dictionary<PlayerController, Checkpoint> playerCheckpoints;

	private void Awake()
	{
		playerCheckpoints = new Dictionary<PlayerController, Checkpoint>();
	}

	private void Start()
	{
		playerCheckpoints.Add(playerController, checkpoints.Last());
		foreach (var checkpoint in checkpoints)
		{
			checkpoint.OnPlayerEntered += CheckCheckPoint;
		}
		playerController.OnRespawnPressed += PlayerController_OnRespawnPressed;
	}

	private void PlayerController_OnRespawnPressed(PlayerController pc)
	{
		pc.transform.position = playerCheckpoints[pc].transform.position;
		pc.transform.rotation = playerCheckpoints[pc].transform.rotation;
		pc.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	private void OnDestroy()
	{
		foreach (var checkpoint in checkpoints)
		{
			checkpoint.OnPlayerEntered -= CheckCheckPoint;
		}
		playerController.OnRespawnPressed -= PlayerController_OnRespawnPressed;
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
