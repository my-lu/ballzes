using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataControl : MonoBehaviour
{
	PlayerProgress playerProgress;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
		LoadPlayerProgress ();
    }

	public void SubmitPlayerScore(int newScore){
		if (newScore > playerProgress.highestScore) {
			playerProgress.highestScore = newScore;
			SavePlayerProgress ();
		}
	}

	public int getHighestPlayerScore(){
		return playerProgress.highestScore;
	}

	void LoadPlayerProgress()
	{
		playerProgress = new PlayerProgress ();

		if(PlayerPrefs.HasKey("highestScore")){
			playerProgress.highestScore = PlayerPrefs.GetInt("highestScore");
		}
	}

	void SavePlayerProgress(){
		PlayerPrefs.SetInt ("highestScore", playerProgress.highestScore);
	}
}
