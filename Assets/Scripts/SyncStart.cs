using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncStart : MonoBehaviour
{
	[SerializeField] Material mat;
	[SerializeField] Transform[] startPositions;
	[SerializeField] float startTime;

	private void Start()
	{
		GameManager.instance.OnStartRacePressed += OnStartRacePressed;
	}

	private void OnDestroy()
	{
		mat.SetColor("_Color", Color.red);
	}

	private void OnStartRacePressed()
	{
		StartCoroutine(StartRaceInX());
	}

	private IEnumerator StartRaceInX()
	{
		for (int i = 0; i < GameManager.instance.playerControllers.Count; i++)
		{
			PlayerController pc = GameManager.instance.playerControllers[i];
			pc.transform.position = startPositions[i].position;
			pc.transform.rotation = startPositions[i].rotation;
			pc.DisableMovement();
		}
		yield return new WaitForSeconds(startTime);
		StartRace();
	}

	private void StartRace()
	{
		MinigameManager.instance.StartManager();
		mat.SetColor("_Color", Color.green);
		for (int i = 0; i < GameManager.instance.playerControllers.Count; i++)
		{
			PlayerController pc = GameManager.instance.playerControllers[i];
			pc.EnableMovement();
		}
	}
}
