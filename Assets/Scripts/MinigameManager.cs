using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MinigameManager : MonoBehaviour
{
	[SerializeField] private float minTimeBetweenGames;
	[SerializeField] private float maxTimeBetweenGames;
	[SerializeField] public float maxMinigameDuration;
	[SerializeField] private List<AMinigame> minigames;
	[SerializeField] private Image inputImage;
	[SerializeField] private Image inputImageBackground;

	private bool canStartMinigame;
	public bool isMinigameRunning;
	private float currentTimeBeforeNextGame;
	private AMinigame currentMinigame;

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
		currentMinigame = minigames[Random.Range(0, minigames.Count)];

		currentMinigame.OnMinigameCleared += OnMinigameCleared;
		currentMinigame.Trigger(inputImage, inputImageBackground);
	}

	private void OnMinigameCleared(PlayerController winner)
	{
		ResetBetweenGames();
		currentMinigame.OnMinigameCleared -= OnMinigameCleared;
	}

	private void ResetBetweenGames()
	{
		isMinigameRunning = false;
		currentTimeBeforeNextGame = Random.Range(minTimeBetweenGames, maxTimeBetweenGames);
	}
}
