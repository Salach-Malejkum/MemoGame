using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI sizeInfoText;
    public TextMeshProUGUI endGameText;
    public Button restartButton;
    public Object[] cardPrefabs;
    public ArrayList spawnedCards;
    public int score = 0;
    public bool gameStarted = false;
    public bool locked = false;
    public bool wait = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        cardPrefabs = Resources.LoadAll("Prefabs/Cards", typeof(GameObject));

        // restartButton.gameObject.SetActive(false);
        // scoreText.enabled = false;
        // sizeInfoText.enabled = false;
        // endGameText.enabled = false;
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

    public void ClearTable()
    {
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
                Destroy(spawnedCards[i] as GameObject);
        }
    }

    public void PrepareGameInfo()
    {
        sizeInfoText.enabled = false;

        restartButton.gameObject.SetActive(true);
        scoreText.SetText("Score: " + score);
        scoreText.gameObject.SetActive(true);
    }

    public void ChooseCards(ref int[] initCards, ref HashSet<int> chosenCards, int rows, int columns)
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

    public void InitateGame(int rows, int columns)
    {
        PrepareGameInfo();
        gameStarted = true;
        int numOfPairs = rows * columns / 2;
        int [] initCards = new int[numOfPairs];
        HashSet<int> chosenCards = new HashSet<int>();
        ChooseCards(ref initCards, ref chosenCards, rows, columns);

        int [] chosenCardsInt = new int [numOfPairs];
        chosenCards.CopyTo(chosenCardsInt);

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int rInt = Random.Range(0, numOfPairs);
                while (initCards[rInt] == 0)
                {
                    rInt = Random.Range(0, numOfPairs);
                }
                spawnedCards.Add(Instantiate(Instance.cardPrefabs[chosenCardsInt[rInt]], new Vector3(800 - 100 * column, 480 - 70 * row, 0), Quaternion.identity));
                initCards[rInt]--;
            }
        }
    }
}
