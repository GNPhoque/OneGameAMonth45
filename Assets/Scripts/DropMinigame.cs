using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropMinigame")]
public class DropMinigame : AMinigame
{
	[SerializeField] private Sprite noButton;
	[SerializeField] private float delayBeforeStart;

	protected override IEnumerator GameSteps()
	{
		gameCompleted = false;
		gameTimeout = false;
		inputImage.gameObject.SetActive(true);
		inputImage.sprite = noButton;

		yield return new WaitForSeconds(delayBeforeStart);

		GameManager.instance.StartCoroutine(CancelOnTimeout());

		//Subscribe to player button
		for (int i = 0; i < GameManager.instance.playerControllers.Count; i++)
		{
			PlayerController player = GameManager.instance.playerControllers[i];
			player.OnInputPressed += CheckMinigameInputConfirm;
		}

		while (!gameTimeout)
		{
			yield return null;
			if (inputOk)
			{
				inputOk = false;
				GiveReward(lastInputPlayer);
			}
		}

		if (gameTimeout)
		{
			OnMinigameCleared?.Invoke(null);
		}

		//Unsubscribe to player button
		for (int i = 0; i < GameManager.instance.playerControllers.Count; i++)
		{
			PlayerController player = GameManager.instance.playerControllers[i];
			player.OnInputPressed -= CheckMinigameInputConfirm;
		}

		inputImage.gameObject.SetActive(false);
	}

	private void GiveReward(PlayerController pc)
	{
		//Rollback all players to last checkpoint
		GameManager.instance.PlayerController_OnRespawnPressed(pc);
		//TODO : apply slow? (balancing question)
	}
}
