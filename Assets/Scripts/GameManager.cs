using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Canvas canvas;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endGameText;
    public Button restartButton;
    public Object[] cardPrefabs;
    public ArrayList spawnedCards;
    public int score = 0;
    public bool gameStarted = false;
    public bool locked = false;
    public bool wait = false;
    [SerializeField] private GameObject mainMenuPanel;

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

    private void Start()
    {
        cardPrefabs = Resources.LoadAll("Prefabs/Cards", typeof(GameObject));
        spawnedCards = new ArrayList();
    }

    private void Update()
    {
        if (!gameStarted)
            return;

        HandleWinLose();

        if (locked && !wait)
        {
            StartCoroutine(CheckCards());
        }
        else if (!wait)
        {
            int clickedCards = CountClickedCards();
            ManageClickedCards(clickedCards);
        }
    }

    private int CountClickedCards()
    {
        int clickedCards = 0;
        foreach (GameObject card in spawnedCards)
        {
            if (card != null && card.gameObject.activeSelf && card.gameObject.GetComponent<CardScript>().IsFlipped)
            {
                clickedCards++;
            }
        }
        return clickedCards;
    }

    private void ManageClickedCards(int clikedCards)
    {
        if (clikedCards >= 2)
            locked = true;
        else
            locked = false;
    }

    IEnumerator CheckCards()
    {
        GameObject card1 = null;
        GameObject card2 = null;
        
        foreach (GameObject card in spawnedCards)
        {
            if (card != null && card.gameObject.GetComponent<CardScript>().IsFlipped)
            {
                if (card1 == null)
                    card1 = card;
                else
                    card2 = card;
            }
        }
        
        wait = true;
        yield return new WaitForSeconds(1.0f);
        card1.gameObject.GetComponent<FlipCard>().Flip();
        card2.gameObject.GetComponent<FlipCard>().Flip();
        if (card1.name == card2.name)
        {
            card1.gameObject.SetActive(false);
            card2.gameObject.SetActive(false);
            UpdateScore(10);
        }
        else
        {
            UpdateScore(-2);
        }
        
        locked = false;
        wait = false;
    }

    void HandleWinLose()
    {      
        int countCards = 0;
        foreach (GameObject card in spawnedCards)
        {
            if (card != null && card.gameObject.activeSelf)
            {
                countCards++;
            }
        }

        if (score <= -10 || countCards == 0)
        {
            string endGameText = score <= -10 ? "YOU LOST" : "YOU WON";
            ShowEndGameText(endGameText);
            gameStarted = false;
            score = 0;
            ClearTable();
            GoToMainMenu();
        }
    }

    private void GoToMainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        restartButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(3.0f);
        endGameText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void ShowEndGameText(string text)
    {
        endGameText.SetText(text);
        endGameText.gameObject.SetActive(true);
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
        restartButton.gameObject.SetActive(true);
        scoreText.SetText("Score: " + score);
        scoreText.gameObject.SetActive(true);
    }

    public void ChooseCards(ref int[] initCards, ref HashSet<int> chosenCards, int rows, int columns)
    {
        int numOfPairs = rows * columns / 2;
        for (int i = 0; i < numOfPairs; i++)
        {
            initCards[i] = 2;
        }

        while (chosenCards.Count != numOfPairs)
        {
            chosenCards.Add(Random.Range(0, numOfPairs));
        }
    }

    public void InitiateGame(int rows, int columns)
    {
        PrepareGameInfo();
        gameStarted = true;
        int numOfPairs = rows * columns / 2;
        int[] initCards = new int[numOfPairs];
        HashSet<int> chosenCards = new HashSet<int>();
        ChooseCards(ref initCards, ref chosenCards, rows, columns);

        int[] chosenCardsInt = new int[numOfPairs];
        chosenCards.CopyTo(chosenCardsInt);

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        int spawnableRows = 2;
        int spawnableColumns = numOfPairs;
        // Calculate the size of the card based on the screen size
        float cardWidth = screenWidth / (spawnableColumns + 1); // +1 for some padding
        float cardHeight = screenHeight / (spawnableRows + 1); // +1 for some padding

        // Calculate the total width and height of all the cards
        float totalCardWidth = spawnableColumns * cardWidth;
        float totalCardHeight = spawnableRows * cardHeight;

        // Calculate the initial position and card offset
        Vector3 initialPosition = new Vector3(cardWidth, totalCardHeight, 0);
        Vector3 cardOffset = new Vector3(cardWidth, -cardHeight, 0);

        for (int row = 0; row < spawnableRows; row++)
        {
            for (int column = 0; column < spawnableColumns; column++)
            {
                int rInt = Random.Range(0, numOfPairs);
                while (initCards[rInt] == 0)
                {
                    rInt = Random.Range(0, numOfPairs);
                }
                Vector3 spawnPosition = initialPosition + (Vector3)(cardOffset * new Vector2(column, row));
                GameObject newCard = Instantiate(Instance.cardPrefabs[chosenCardsInt[rInt]], spawnPosition, Quaternion.identity, canvas.transform) as GameObject;
                spawnedCards.Add(newCard);
                initCards[rInt]--;
            }
        }
    }

    private void UpdateScore(int val)
    {
        score += val;
        scoreText.SetText("Score: " + score);
    }
}
