using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MinigameManager : MonoBehaviour
{
	[SerializeField] float minTimeBetweenGames;
	[SerializeField] float maxTimeBetweenGames;
	[SerializeField] List<AMinigame> minigames;
	[SerializeField] Image inputImage;
	[SerializeField] Image inputImageBackground;

	private bool canStartMinigame;
	public bool isMinigameRunning;
	private float currentTimeBeforeNextGame;

	public static MinigameManager instance;

	private void Awake()
	{
		if(instance != null)
		{
			return;
		}

		instance = this;
	}

	private void Update()
	{
		if (!canStartMinigame)
		{
			return;
		}

		if (!isMinigameRunning)
		{
			currentTimeBeforeNextGame -= Time.deltaTime;
			if (currentTimeBeforeNextGame <= 0f)
			{
				StartRandomMinigame();
			}
		}
	}

	public void StartManager()
	{
		canStartMinigame = true;
		ResetBetweenGames();
	}

	private void StartRandomMinigame()
	{
		isMinigameRunning = true;
		AMinigame game = minigames[Random.Range(0, minigames.Count)];

		game.OnMinigameCleared += OnMinigameCleared;
		game.Trigger(inputImage, inputImageBackground);
	}

	private void OnMinigameCleared(PlayerController winner)
	{
		ResetBetweenGames();
	}

	private void ResetBetweenGames()
	{
		isMinigameRunning = false;
		currentTimeBeforeNextGame = Random.Range(minTimeBetweenGames, maxTimeBetweenGames);
	}
}
