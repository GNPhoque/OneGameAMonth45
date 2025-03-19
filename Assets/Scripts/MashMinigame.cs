using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MashMinigame")]
public class MashMinigame : AMinigame
{
    [SerializeField] private Sprite aButtonOn;
    [SerializeField] private Sprite aButtonOff;
    [SerializeField] private float animationStatesDuration;
    [SerializeField] private float rewardBoostAmount;
    [SerializeField] private float rewardBoostDuration;
    [SerializeField] private int minMashCount;
    [SerializeField] private int maxMashCount;

    private int currentGameMashCount;
    private int[] playerMashCount;

    protected override IEnumerator GameSteps()
    {
        gameCompleted = false;
        gameTimeout = false;
        inputImage.gameObject.SetActive(true);

        GameManager.instance.StartCoroutine(CancelOnTimeout());

		//Set mash count
		currentGameMashCount = Random.Range(minMashCount, maxMashCount);

        //Apply mash count to every player
        playerMashCount = new int[GameManager.instance.playerControllers.Count];
        for (int i = 0; i < playerMashCount.Length; i++)
        {
            playerMashCount[i] = currentGameMashCount;
        }

        //Subscribe to player button
		for (int i = 0; i < GameManager.instance.playerControllers.Count; i++)
        {
            PlayerController player = GameManager.instance.playerControllers[i];
            player.OnConfirmPressed += CheckMinigameInputConfirm;
        }
        
        //Start Game
		GameManager.instance.StartCoroutine(AnimateInputImage());

        while (!gameCompleted && !gameTimeout)
        {
            yield return null;
            if (inputOk)
			{
                inputOk = false;
				int index = GameManager.instance.playerControllers.IndexOf(lastInputPlayer);
				playerMashCount[index]--;

                if(playerMashCount[index] <= 0)
                {
                    gameCompleted = true;
                    OnMinigameCleared?.Invoke(lastInputPlayer);
                    GameManager.instance.StartCoroutine(GiveReward(lastInputPlayer));
                }
			}
        }

        if (gameTimeout)
        {
			//Malus?
			OnMinigameCleared?.Invoke(null);
		}

		//Subscribe to player button
		for (int i = 0; i < GameManager.instance.playerControllers.Count; i++)
		{
			PlayerController player = GameManager.instance.playerControllers[i];
			player.OnConfirmPressed -= CheckMinigameInputConfirm;
		}

		inputImage.gameObject.SetActive(false);
	}

	private IEnumerator AnimateInputImage()
    {
        WaitForSeconds wfs = new WaitForSeconds(animationStatesDuration);

        while (MinigameManager.instance.isMinigameRunning)
        {
            inputImage.sprite = aButtonOn;
            yield return wfs;
            inputImage.sprite = aButtonOff;
            yield return wfs;
        }
    }

    private IEnumerator GiveReward(PlayerController pc)
    {
        //Boost winner speed
        pc.ChangeAccel(rewardBoostAmount);
        yield return new WaitForSeconds(rewardBoostDuration);
		pc.ChangeAccel(-rewardBoostAmount);
	}
}
