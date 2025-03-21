using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncStart : MonoBehaviour
{
	[SerializeField] Material mat;
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
		yield return new WaitForSeconds(startTime);
		StartRace();
	}

	private void StartRace()
	{
		mat.SetColor("_Color", Color.green);
	}
}
