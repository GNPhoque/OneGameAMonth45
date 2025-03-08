using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] private List<Checkpoint> checkpoints;
	[SerializeField] private PlayerController playerController;

	[SerializeField] private Image minigameInputImage;
	[SerializeField] private GameObject fuelMinigame;
	[SerializeField] private Image fuelFillImage;
	[SerializeField] private Sprite stickLeft;
	[SerializeField] private Sprite stickUp;
	[SerializeField] private Sprite stickRight;
	[SerializeField] private Sprite stickDown;
	[SerializeField] private Sprite buttonA;

	private Dictionary<PlayerController, Checkpoint> playerCheckpoints;
	bool inputOk = false;

	private void Awake()
	{
		playerCheckpoints = new Dictionary<PlayerController, Checkpoint>();
	}

	private void Start()
	{
		playerCheckpoints.Add(playerController, checkpoints.Last());
		foreach (var checkpoint in checkpoints)
		{
			checkpoint.OnPlayerEntered += CheckCheckPoint;
		}
		playerController.OnRespawnPressed += PlayerController_OnRespawnPressed;
	}

	private void PlayerController_OnRespawnPressed(PlayerController pc)
	{
		pc.transform.position = playerCheckpoints[pc].transform.position;
		pc.transform.rotation = playerCheckpoints[pc].transform.rotation;
		pc.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	private void OnDestroy()
	{
		foreach (var checkpoint in checkpoints)
		{
			checkpoint.OnPlayerEntered -= CheckCheckPoint;
		}
		playerController.OnRespawnPressed -= PlayerController_OnRespawnPressed;
	}

	private void CheckCheckPoint(PlayerController pc, Checkpoint checkpoint)
	{
		print($"{pc.name} passed a checkpoint {checkpoint.name}");

		Checkpoint previous = playerCheckpoints[pc];

		//Only valid checkpoint is the first one
		if(previous == checkpoints.Last())
		{
			if(checkpoint != checkpoints[0])
			{
				print($"{pc.name} went through the wrong checkpoint");
				return;
			}
			playerCheckpoints[pc] = checkpoint;
		}
		else
		{
			if (checkpoint != checkpoints[checkpoints.IndexOf(previous) + 1])
			{
				print($"{pc.name} went through the wrong checkpoint");
				return;
			}
			playerCheckpoints[pc] = checkpoint;
		}

		if(checkpoint == checkpoints.Last())
		{
			print($"{pc.name} finished a lap");
			StartCoroutine(TriggerUnscrewMinigame());
		}
	}

	//Minigame to remove the wheels
	private IEnumerator TriggerUnscrewMinigame()
	{
		for (int i = 0; i < 3; i++)
		{
			minigameInputImage.sprite = stickUp;
			playerController.OnDirectionChanged += CheckMinigameInputUp;
			yield return new WaitUntil(() => inputOk == true);
			inputOk = false;
			playerController.OnDirectionChanged -= CheckMinigameInputUp;

			minigameInputImage.sprite = stickLeft;
			playerController.OnDirectionChanged += CheckMinigameInputLeft;
			yield return new WaitUntil(() => inputOk == true);
			inputOk = false;
			playerController.OnDirectionChanged -= CheckMinigameInputLeft;

			minigameInputImage.sprite = stickDown;
			playerController.OnDirectionChanged += CheckMinigameInputDown;
			yield return new WaitUntil(() => inputOk == true);
			inputOk = false;
			playerController.OnDirectionChanged -= CheckMinigameInputDown;

			minigameInputImage.sprite = stickRight;
			playerController.OnDirectionChanged += CheckMinigameInputRight;
			yield return new WaitUntil(() => inputOk == true);
			inputOk = false;
			playerController.OnDirectionChanged -= CheckMinigameInputRight; 
		}

		minigameInputImage.sprite = null;

		StartCoroutine(TriggerFuelMinigame());
	}

	private void CheckMinigameInputUp(Vector2 input)
	{
		inputOk = input.y > .7f;
	}

	private void CheckMinigameInputLeft(Vector2 input)
	{
		inputOk = input.x < -.7f;
	}

	private void CheckMinigameInputDown(Vector2 input)
	{
		inputOk = input.y < -.7f;
	}

	private void CheckMinigameInputRight(Vector2 input)
	{
		inputOk = input.x > .7f;
	}

	private void CheckMinigameInputConfirm(PlayerController pc)
	{
		inputOk = true;
	}

	private IEnumerator TriggerFuelMinigame()
	{
		//Setup
		minigameInputImage.sprite = buttonA;
		fuelMinigame.SetActive(true);
		fuelFillImage.fillAmount = 0f;
		float target = 0.7f;

		//Wait for input
		playerController.OnConfirmPressed += CheckMinigameInputConfirm;
		Coroutine fillAnimCoroutine = StartCoroutine(AnimateGasTankFill());
		yield return new WaitUntil(() => inputOk == true);
		inputOk = false;
		minigameInputImage.sprite = null;

		//Get result
		playerController.OnConfirmPressed -= CheckMinigameInputConfirm;
		StopCoroutine(fillAnimCoroutine);
		fillAnimCoroutine = StartCoroutine(AnimateGasTankFillEnd(target));
		yield return new WaitUntil(() => inputOk == true);
		inputOk = false;

		StopCoroutine(fillAnimCoroutine);
		fuelMinigame.SetActive(false);

		StartCoroutine(TriggerScrewMinigame());
	}

	private IEnumerator AnimateGasTankFill()
	{
		bool increasing = true;
		while (true)
		{
			yield return null;
			fuelFillImage.fillAmount += Time.deltaTime * (increasing ? 1f : -1f);
			if (fuelFillImage.fillAmount >= 1)
			{
				increasing = false;
			}
			else if (fuelFillImage.fillAmount <= 0)
			{
				increasing = true;
			}
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
				inputOk = true;
			}
			else if (increasing == false && fuelFillImage.fillAmount <= target)
			{
				inputOk = true;
			}
		}
	}
	private IEnumerator TriggerScrewMinigame()
	{
		for (int i = 0; i < 3; i++)
		{
			minigameInputImage.sprite = stickUp;
			playerController.OnDirectionChanged += CheckMinigameInputUp;
			yield return new WaitUntil(() => inputOk == true);
			inputOk = false;
			playerController.OnDirectionChanged -= CheckMinigameInputUp;

			minigameInputImage.sprite = stickRight;
			playerController.OnDirectionChanged += CheckMinigameInputRight;
			yield return new WaitUntil(() => inputOk == true);
			inputOk = false;
			playerController.OnDirectionChanged -= CheckMinigameInputRight;

			minigameInputImage.sprite = stickDown;
			playerController.OnDirectionChanged += CheckMinigameInputDown;
			yield return new WaitUntil(() => inputOk == true);
			inputOk = false;
			playerController.OnDirectionChanged -= CheckMinigameInputDown;

			minigameInputImage.sprite = stickLeft;
			playerController.OnDirectionChanged += CheckMinigameInputLeft;
			yield return new WaitUntil(() => inputOk == true);
			inputOk = false;
			playerController.OnDirectionChanged -= CheckMinigameInputLeft; 
		}

		minigameInputImage.sprite = null;
	}
}
