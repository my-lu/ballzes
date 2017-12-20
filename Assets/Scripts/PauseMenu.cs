using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public void SpeedGame()
	{
		GameManager.instance.SpeedUpGame();
	}

    public void PauseGame()
    {
        GameManager.instance.OpenPauseMenu();
    }

    public void ResumeGame()
    {
        GameManager.instance.ClosePauseMenu();
    }

    public void RestartGame()
    {
        GameManager.instance.RestartGame();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
