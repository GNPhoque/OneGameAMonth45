using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscrewMinigame : AMinigame
{
	//Minigame to remove the wheels
	protected override IEnumerator GameSteps()
	{
		//for (int i = 0; i < 3; i++)
		//{
		//	playerController.playerUI.minigameInputImage.sprite = playerController.playerUI.stickUp;
		//	playerController.OnDirectionChanged += CheckMinigameInputUp;
		//	yield return new WaitUntil(() => inputOk == true);
		//	inputOk = false;
		//	playerController.OnDirectionChanged -= CheckMinigameInputUp;

		//	playerController.playerUI.minigameInputImage.sprite = playerController.playerUI.stickLeft;
		//	playerController.OnDirectionChanged += CheckMinigameInputLeft;
		//	yield return new WaitUntil(() => inputOk == true);
		//	inputOk = false;
		//	playerController.OnDirectionChanged -= CheckMinigameInputLeft;

		//	playerController.playerUI.minigameInputImage.sprite = playerController.playerUI.stickDown;
		//	playerController.OnDirectionChanged += CheckMinigameInputDown;
		//	yield return new WaitUntil(() => inputOk == true);
		//	inputOk = false;
		//	playerController.OnDirectionChanged -= CheckMinigameInputDown;

		//	playerController.playerUI.minigameInputImage.sprite = playerController.playerUI.stickRight;
		//	playerController.OnDirectionChanged += CheckMinigameInputRight;
		//	yield return new WaitUntil(() => inputOk == true);
		//	inputOk = false;
		//	playerController.OnDirectionChanged -= CheckMinigameInputRight;
		//}

		//playerController.playerUI.minigameInputImage.sprite = null;
		yield break;
	}
}
