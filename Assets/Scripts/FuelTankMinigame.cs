using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelTankMinigame : AMinigame
{
	float target = 0.7f;

	protected override IEnumerator GameSteps()
	{
		//Setup
		playerController.playerUI.minigameInputImage.sprite = playerController.playerUI.buttonA;

		//Wait for input
		playerController.playerUI.StartTankFillAnimation();
		playerController.OnConfirmPressed += CheckMinigameInputConfirm;
		yield return new WaitUntil(() => inputOk == true);
		playerController.OnConfirmPressed -= CheckMinigameInputConfirm;
		inputOk = false;
		playerController.playerUI.minigameInputImage.sprite = null;
		playerController.playerUI.StopTankFillAnimation();

		//Get result
		playerController.playerUI.StartTankFillSlowAnimation(target);
		playerController.playerUI.OnFuelTankSlowFillEnded += OnTankSlowFillComplete;
		yield return new WaitUntil(() => inputOk == true);
		inputOk = false;
		playerController.playerUI.OnFuelTankSlowFillEnded -= OnTankSlowFillComplete;
		playerController.playerUI.StopTankFillAnimation();
		playerController.playerUI.EndFuelTankMinigame();
	}

	private void OnTankSlowFillComplete()
	{
		inputOk = true;
	}
}
