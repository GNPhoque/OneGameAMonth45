using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ReflexMinigame")]
public class ReflexMinigame : AMinigame
{
	[SerializeField] private Sprite aButtonOn;

	protected override IEnumerator GameSteps()
	{
		gameCompleted = false;
		gameTimeout = false;
		inputImage.gameObject.SetActive(true);
		//Todo : background reflex

		GameManager.instance.StartCoroutine(CancelOnTimeout());

		//Subscribe to player button
		for (int i = 0; i < GameManager.instance.playerControllers.Count; i++)
		{
			PlayerController player = GameManager.instance.playerControllers[i];
			player.OnConfirmPressed += CheckMinigameInputConfirm;
		}

		while (!gameCompleted && !gameTimeout)
		{
			yield return null;
			if (inputOk)
			{
				inputOk = false;
				gameCompleted = true;

				OnMinigameCleared?.Invoke(lastInputPlayer);
				GiveReward(lastInputPlayer);
			}
		}

		if (gameTimeout)
		{
			//Malus?
			OnMinigameCleared?.Invoke(null);
			GiveReward(null);
		}

		//Subscribe to player button
		for (int i = 0; i < GameManager.instance.playerControllers.Count; i++)
		{
			PlayerController player = GameManager.instance.playerControllers[i];
			player.OnConfirmPressed -= CheckMinigameInputConfirm;
		}

		inputImage.gameObject.SetActive(false);
	}

	private void GiveReward(PlayerController pc)
	{
		//Rollback all players to last checkpoint
		foreach (var item in GameManager.instance.playerControllers)
		{
			if (item == pc)
			{
				continue;
			}

			GameManager.instance.PlayerController_OnRespawnPressed(item);
		}
	}
}
