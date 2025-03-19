using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MashMinigame")]
public class MashMinigame : AMinigame
{
    [SerializeField] private Sprite aButtonOn;
    [SerializeField] private Sprite aButtonOff;
    [SerializeField] private float animationStatesDuration;
    [SerializeField] private int minMashCount;
    [SerializeField] private int maxMashCount;

    private int currentGameMashCount;
    private int[] playerMashCount;

    private bool gameCompleted;

    protected override IEnumerator GameSteps()
    {
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

        while (!gameCompleted)
        {
            yield return null;
            if (inputOk)
			{
				int index = GameManager.instance.playerControllers.IndexOf(lastInputPlayer);
				playerMashCount[index]--;

                if(playerMashCount[index] <= 0)
                {
                    gameCompleted = true;
                    OnMinigameCleared?.Invoke(lastInputPlayer);
                }
			}
        }
    }

    private IEnumerator AnimateInputImage()
    {
        WaitForSeconds wfs = new WaitForSeconds(animationStatesDuration);

        while (MinigameManager.instance.isMinigameRunning)
        {
            inputImage.sprite = aButtonOn;
            yield return wfs;
            inputImage.sprite = aButtonOn;
            yield return wfs;
        }
    }
}
