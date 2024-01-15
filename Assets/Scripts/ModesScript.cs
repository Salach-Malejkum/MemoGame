using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModesScript : MonoBehaviour
{
    [SerializeField] private GameObject chooseGamePanel;

    public void Mode2x2()
    {
        chooseGamePanel.SetActive(false);
        GameManager.Instance.InitiateGame(2, 2);
    }
    
    public void Mode2x4()
    {
        chooseGamePanel.SetActive(false);
        GameManager.Instance.InitiateGame(2, 4);
    }

    public void Mode4x4()
    {
        chooseGamePanel.SetActive(false);
        GameManager.Instance.InitiateGame(4, 4);
    }
}
