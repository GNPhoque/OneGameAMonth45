using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Minigames
{
	Unscrew, 
	FuelTank,
	Screw,
}

public abstract class AMinigame
{
	protected bool inputOk = false;
	protected bool minigameOk = false;
	public PlayerController playerController;

	public IEnumerator Trigger()
	{
		inputOk = false;
		minigameOk = false;

		yield return GameManager.instance.StartCoroutine(GameSteps());
	}

	protected abstract IEnumerator GameSteps();

	protected void CheckMinigameInputUp(PlayerController pc, Vector2 input)
	{
		if (pc != playerController)
		{
			return;
		}

		inputOk = input.y > .7f;
	}

	protected void CheckMinigameInputLeft(PlayerController pc, Vector2 input)
	{
		if (pc != playerController)
		{
			return;
		}

		inputOk = input.x < -.7f;
	}

	protected void CheckMinigameInputDown(PlayerController pc, Vector2 input)
	{
		if (pc != playerController)
		{
			return;
		}
		inputOk = input.y < -.7f;
	}

	protected void CheckMinigameInputRight(PlayerController pc, Vector2 input)
	{
		if (pc != playerController)
		{
			return;
		}

		inputOk = input.x > .7f;
	}

	protected void CheckMinigameInputConfirm(PlayerController pc)
	{
		if(pc != playerController)
		{
			return;
		}

		inputOk = true;
	}
}
