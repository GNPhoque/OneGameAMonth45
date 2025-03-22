using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MinigameManager : MonoBehaviour
{
	[SerializeField] private Image inputImage;
	[SerializeField] private Image inputImageBackground;
	[SerializeField] private Image minigameTimerFill;
	[SerializeField] private TextMeshProUGUI timerText;

	[SerializeField] private float minTimeBetweenGames;
	[SerializeField] private float maxTimeBetweenGames;
	[SerializeField] public float maxMinigameDuration;
	[SerializeField] private List<AMinigame> minigames;

	private bool canStartMinigame;
	public bool isMinigameRunning;
	private float currentTimeBeforeNextGame;
	private float timeBeforeNextMinigame;
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
			minigameTimerFill.fillAmount = currentTimeBeforeNextGame / timeBeforeNextMinigame;

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
		timerText.text = currentMinigame.GetGameDescription();
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
		timeBeforeNextMinigame = Random.Range(minTimeBetweenGames, maxTimeBetweenGames);
		currentTimeBeforeNextGame = timeBeforeNextMinigame;
		timerText.text = "Time before next minigame";
	}
}
