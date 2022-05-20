using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI sizeInfoText;
    public TextMeshProUGUI endGameText;
    public Button newGameButton;
    public Button restartButton;
    public Button mode2x2Button;
    public Button mode2x4Button;
    public Button mode4x4Button;
    public Object[] cardPrefabs;
    ArrayList spawnedCards;

    private int score = 0;
    private bool gameStarted = false;
    public bool locked = false;
    public bool wait = false;
    // Start is called before the first frame update
    void Start()
    {
        cardPrefabs = Resources.LoadAll("Prefabs/Cards", typeof(GameObject));
        
        newGameButton.onClick.AddListener(ChooseMode);
        restartButton.onClick.AddListener(RestarGame);
        mode2x2Button.onClick.AddListener(Mode2x2);
        mode2x4Button.onClick.AddListener(Mode2x4);
        mode4x4Button.onClick.AddListener(Mode4x4);

        restartButton.gameObject.SetActive(false);
        mode2x2Button.gameObject.SetActive(false);
        mode2x4Button.gameObject.SetActive(false);
        mode4x4Button.gameObject.SetActive(false);
        scoreText.enabled = false;
        sizeInfoText.enabled = false;
        endGameText.enabled = false;
        spawnedCards = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            CheckWinLose();

            scoreText.SetText("Score: " + score);
            if (locked && !wait)
            {
               StartCoroutine(CheckCards());
            }

            if (!wait)
            {
                int clikedCards = 0;
                for (int i = 0; i < spawnedCards.Count; i++)
                {
                    if (spawnedCards[i] != null)
                    {
                        GameObject obj = spawnedCards[i] as GameObject;
                        if (!obj.transform.GetChild(1).gameObject.gameObject.activeSelf && obj.gameObject.activeSelf)
                        {
                            clikedCards++;
                            if (clikedCards == 2)
                            {
                                locked = true;
                                break;
                            }
                        }
                    }
                }
                if (clikedCards >= 2)
                    locked = true;
                else 
                    locked = false;
            }
        }
    }

    IEnumerator CheckCards()
    {
        GameObject card1 = null;
        GameObject card2 = null;
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
            {
                GameObject tmp = spawnedCards[i] as GameObject;
                if (!tmp.transform.GetChild(1).gameObject.gameObject.activeSelf && tmp.gameObject.activeSelf)
                {
                    if (card1 == null)
                    {
                        card1 = spawnedCards[i] as GameObject;
                    }
                    else
                    {
                        card2 = spawnedCards[i] as GameObject;
                    }
                }
            }
        }
        
        Debug.Log("Card1: " + card1.name);
        Debug.Log("Card2: " + card2.name);
        Debug.Log(card1.name == card2.name);
        if (card1.name == card2.name)
        {
            wait = true;
            yield return new WaitForSeconds(0.5f);
            card1.gameObject.SetActive(false);
            card2.gameObject.SetActive(false);
            score += 10;
            locked = false;
            wait = false;
        }
        else
        {
            wait = true;
            yield return new WaitForSeconds(1.0f);
            card1.transform.GetChild(1).gameObject.SetActive(true);
            card2.transform.GetChild(1).gameObject.SetActive(true);
            score -= 2;
            locked = false;
            wait = false;
        }
        
    }

    void CheckWinLose()
    {
        if (score <= -10)
        {
            endGameText.SetText("YOU LOST");
            endGameText.enabled = true;
            gameStarted = false;
            ClearTable();
        }
        int countCards = 0;
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
            {
                GameObject tmp = spawnedCards[i] as GameObject;
                if (tmp.gameObject.activeSelf)
                {
                    countCards++;
                }
            }
        }

        if (countCards == 0)
        {
            endGameText.SetText("YOU WON");
            endGameText.enabled = true;
            gameStarted = false;
            ClearTable();
        }
    }

    void ChooseMode()
    {
        spawnedCards = new ArrayList();
        newGameButton.gameObject.SetActive(false);
        sizeInfoText.enabled = true;
        mode2x2Button.gameObject.SetActive(true);
        mode2x4Button.gameObject.SetActive(true);
        mode4x4Button.gameObject.SetActive(true);
    }

    void ClearTable()
    {
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
                Destroy(spawnedCards[i] as GameObject);
        }
    }

    public void RestarGame()
    {
        ClearTable();
        endGameText.enabled = false;
        gameStarted = false;
        score = 0;
        ChooseMode();
    }

    void PrepareGameInfo()
    {
        mode2x2Button.gameObject.SetActive(false);
        mode2x4Button.gameObject.SetActive(false);
        mode4x4Button.gameObject.SetActive(false);
        sizeInfoText.enabled = false;

        restartButton.gameObject.SetActive(true);
        scoreText.SetText("Score: " + score);
        scoreText.enabled = true;
    }

    void ChooseCards(ref int[] initCards, ref HashSet<int> chosenCards, int rows, int columns)
    {
        for (int i = 0; i < rows * columns / 2; i++)
        {
            initCards[i] = 2;
        }
        
        while (chosenCards.Count != rows * columns / 2)
        {
            chosenCards.Add(Random.Range(0, 8));
        }
    }

    void Mode2x2()
    {
        PrepareGameInfo();
        gameStarted = true;
        int [] initCards = new int[2];
        HashSet<int> chosenCards = new HashSet<int>();
        ChooseCards(ref initCards, ref chosenCards, 2, 2);

        int [] chosenCardsInt = new int [2];
        chosenCards.CopyTo(chosenCardsInt);

        for (int row = 0; row < 2; row++)
        {
            for (int column = 0; column < 2; column++)
            {
                int rInt = Random.Range(0, 2);
                while (initCards[rInt] == 0)
                {
                    rInt = Random.Range(0, 2);
                }
                spawnedCards.Add(Instantiate(cardPrefabs[chosenCardsInt[rInt]], new Vector3(800 - 100 * column, 480 - 70 * row, 0), Quaternion.identity));
                initCards[rInt]--;
            }
        }
    }
    
    void Mode2x4()
    {
        PrepareGameInfo();
        gameStarted = true;
        int [] initCards = new int[4];
        HashSet<int> chosenCards = new HashSet<int>();
        ChooseCards(ref initCards, ref chosenCards, 2, 4);

        int [] chosenCardsInt = new int [4];
        chosenCards.CopyTo(chosenCardsInt);

        for (int row = 0; row < 2; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                int rInt = Random.Range(0, 4);
                while (initCards[rInt] == 0)
                {
                    rInt = Random.Range(0, 4);
                }
                spawnedCards.Add(Instantiate(cardPrefabs[chosenCardsInt[rInt]], new Vector3(800 - 100 * column, 480 - 70 * row, 0), Quaternion.identity));
                initCards[rInt]--;
            }
        }
    }

    void Mode4x4()
    {
        PrepareGameInfo();
        gameStarted = true;
        int [] initCards = new int[8];
        HashSet<int> chosenCards = new HashSet<int>();
        ChooseCards(ref initCards, ref chosenCards, 4, 4);

        int [] chosenCardsInt = new int [8];
        chosenCards.CopyTo(chosenCardsInt);

        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                int rInt = Random.Range(0, 8);
                while (initCards[rInt] == 0)
                {
                    rInt = Random.Range(0, 8);
                }
                spawnedCards.Add(Instantiate(cardPrefabs[chosenCardsInt[rInt]], new Vector3(800 - 100 * column, 480 - 70 * row, 0), Quaternion.identity));
                initCards[rInt]--;
            }
        }
    }
}
