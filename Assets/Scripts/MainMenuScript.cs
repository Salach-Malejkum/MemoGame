using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject chooseGamePanel;
    [SerializeField] private GameObject mainMenuPanel;

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        chooseGamePanel.SetActive(true);
        GameManager.Instance.spawnedCards = new ArrayList();
    }

    public void Settings()
    {
        Debug.Log("Settings");
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    public void RestarGame()
    {
        GameManager.Instance.ClearTable();
        GameManager.Instance.endGameText.enabled = false;
        GameManager.Instance.gameStarted = false;
        GameManager.Instance.score = 0;
        StartGame();
    }
}
