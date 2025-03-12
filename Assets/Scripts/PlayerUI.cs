using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[SerializeField] public Sprite stickLeft;
	[SerializeField] public Sprite stickUp;
	[SerializeField] public Sprite stickRight;
	[SerializeField] public Sprite stickDown;
	[SerializeField] public Sprite buttonA;

	[SerializeField] public Image minigameInputImage;
	[SerializeField] private GameObject fuelMinigame;
	[SerializeField] private Image fuelFillImage;

	private Coroutine tankFillAnimation;

	public event Action OnFuelTankSlowFillEnded;

	public void StartTankFillAnimation()
	{
		tankFillAnimation = StartCoroutine(AnimateGasTankFill());
	}

	public void StartTankFillSlowAnimation(float target)
	{
		tankFillAnimation = StartCoroutine(AnimateGasTankFillEnd(target));
	}

	public void StopTankFillAnimation()
	{
		StopCoroutine(tankFillAnimation);
	}

	public void EndFuelTankMinigame()
	{
		fuelMinigame.SetActive(false);
	}

	private IEnumerator AnimateGasTankFill()
	{
		fuelFillImage.fillAmount = 0f;
		fuelMinigame.SetActive(true);
		bool increasing = true;

		while (true)
		{
			if (fuelFillImage.fillAmount >= 1)
			{
				increasing = false;
			}
			else if (fuelFillImage.fillAmount <= 0)
			{
				increasing = true;
			}
			yield return null;
			fuelFillImage.fillAmount += Time.deltaTime * (increasing ? 1f : -1f);
		}
	}

	private IEnumerator AnimateGasTankFillEnd(float target)
	{
		bool increasing = fuelFillImage.fillAmount <= target;
		while (true)
		{
			yield return null;
			fuelFillImage.fillAmount += Time.deltaTime * (increasing ? .3f : -.3f);
			if (increasing && fuelFillImage.fillAmount >= target)
			{
				OnFuelTankSlowFillEnded?.Invoke();
			}
			else if (increasing == false && fuelFillImage.fillAmount <= target)
			{
				OnFuelTankSlowFillEnded?.Invoke();
			}
		}
	}
}
