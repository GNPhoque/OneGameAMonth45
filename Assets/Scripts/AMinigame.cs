using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AMinigame : ScriptableObject
{
	protected bool inputOk = false;
	protected bool minigameOk = false;
	protected bool gameTimeout = false;
	protected bool gameCompleted = false;
	[SerializeField] [TextArea] protected string gameDescription;

	protected Image inputImage;
	protected Image inputImageBackground;
	protected PlayerController lastInputPlayer;

	public Action<PlayerController> OnMinigameCleared;

	public string GetGameDescription() => gameDescription;

	public void Trigger(Image input, Image background)
	{
		inputImage = input;
		inputImageBackground = background;

		inputOk = false;
		minigameOk = false;

		Debug.Log($"Starting minigame {nameof(this.GetType)}");
		GameManager.instance.StartCoroutine(GameSteps());
	}

	protected abstract IEnumerator GameSteps();

	protected void CheckMinigameInputUp(PlayerController pc, Vector2 input)
	{
		lastInputPlayer = pc;
		inputOk = input.y > .7f;
	}

	protected void CheckMinigameInputLeft(PlayerController pc, Vector2 input)
	{
		lastInputPlayer = pc;
		inputOk = input.x < -.7f;
	}

	protected void CheckMinigameInputDown(PlayerController pc, Vector2 input)
	{
		lastInputPlayer = pc;
		inputOk = input.y < -.7f;
	}

	protected void CheckMinigameInputRight(PlayerController pc, Vector2 input)
	{
		lastInputPlayer = pc;
		inputOk = input.x > .7f;
	}

	protected void CheckMinigameInputConfirm(PlayerController pc)
	{
		lastInputPlayer = pc;
		inputOk = true;
	}

	protected IEnumerator CancelOnTimeout()
	{
		yield return new WaitForSeconds(MinigameManager.instance.maxMinigameDuration);
		gameTimeout = true;
	}

	protected void TriggerGameCleared()
	{
		OnMinigameCleared?.Invoke(lastInputPlayer);
	}
}
