using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
	public void NewGame()
	{
		GameManager.instance.RestartGame();
	}

	public void MainMenu()
	{
		SceneManager.LoadScene(0);
	}
}
