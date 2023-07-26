using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    public List<string> superHeroNames = new() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    public List<string> cardNames = new() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    public GameObject cardPrefab;
    //public GameObject superHeroPrefab;

    public Transform playerHandTransform;
    public Transform opponentHandTransform;
    public Transform deck;

    public TMP_Text winText;
    public TMP_Text gamePanelText;

    [SerializeField] private List<string> playerSuperCards = new();
    [SerializeField] private List<string> opponentSuperCards = new();

    [SerializeField] private List<string> playerNormalCards = new();
    [SerializeField] private List<string> opponentNormalCards = new();

    [SerializeField] private List<string> playerHand = new();
    [SerializeField] private List<string> opponentHand = new();

    [SerializeField] private List<string> deckCards = new();
    [SerializeField] private bool gameIsOver = false;

    [SerializeField] private bool isPlayerTurn = true;

    public List<GameObject> playerSuperCardsObj;
    public List<GameObject> playerNormalCardsObj;
    public List<GameObject> opponentSuperCardObj;
    public List<GameObject> opponentNormalCardsObj;

    //public Card selectedCard;
    public Button drawBtn;
    public GameObject gamePanel;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeGame();

        if(photonView.IsMine)
        {
            Debug.Log("photon mine");

            /*for(int i = 0; i < playerNormalCardsObj.Count; i++) 
            { 
                playerSuperCardsObj[i]. = null; 
            }*/
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    private void InitializeGame()
    {
        Shuffle(superHeroNames);
        Shuffle(cardNames);

        for (int i = 0; i < 3; i++)
        {
            playerSuperCards.Add(superHeroNames[i]);
            opponentSuperCards.Add(superHeroNames[i + 3]);
        }

        for (int i = 0; i < 5; i++)
        {
            playerNormalCards.Add(cardNames[i]);
            opponentNormalCards.Add(cardNames[i + 5]);
        }

        deckCards.AddRange(cardNames.GetRange(0, cardNames.Count));

        foreach (string cardName in deckCards)
        {
            GameObject cardObj = Instantiate(cardPrefab, deck);
            Card card = cardObj.GetComponent<Card>();
            card.CardType = CardType.NormalCard;
            card.Initialize(cardName);
            cardObj.SetActive(false);
        }

        foreach (string cardName in playerSuperCards)
        {
            GameObject cardObj = Instantiate(cardPrefab, playerHandTransform);
            Card card = cardObj.GetComponent<Card>();
            playerSuperCardsObj.Add(cardObj);
            card.CardType = CardType.SuperCard;
            card.Initialize(cardName);
        }

        foreach (string cardName in playerNormalCards)
        {
            for (int i = 0; i < playerSuperCards.Count; i++)
            {
                if (cardName != playerSuperCards[i])
                {
                    GameObject cardObj = Instantiate(cardPrefab, playerHandTransform);
                    Card card = cardObj.GetComponent<Card>();
                    playerNormalCardsObj.Add(cardObj);
                    card.CardType = CardType.NormalCard;
                    card.Initialize(cardName);
                }
                break;
            }
        }

        foreach (string cardName in opponentSuperCards)
        {
            GameObject cardObj = Instantiate(cardPrefab, opponentHandTransform);
            Card card = cardObj.GetComponent<Card>();
            opponentSuperCardObj.Add(cardObj);
            card.CardType = CardType.SuperCard;
            card.Initialize(cardName);
        }

        foreach (string cardName in opponentNormalCards)
        {
            for (int i = 0; i < opponentSuperCards.Count; i++)
            {
                if (cardName != opponentSuperCards[i])
                {
                    GameObject cardObj = Instantiate(cardPrefab, opponentHandTransform);
                    Card card = cardObj.GetComponent<Card>();
                    opponentNormalCardsObj.Add(cardObj);
                    card.CardType = CardType.NormalCard;
                    card.Initialize(cardName);
                }
                break;
            }
        }
    }

    IEnumerator MoveCard(GameObject card, Transform destination, float t)
    {
        card.transform.DOLocalMove(destination.localPosition, t);
        yield return new WaitForSeconds(t);
        OnCompleteTransform(card, destination);
    }

    void OnCompleteTransform(GameObject card, Transform destination)
    {
        card.transform.SetParent(destination);
    }

    public void DrawCard()
    {
        if (isPlayerTurn)
        {
            if (deckCards.Count > 0)
            {
                int randomCardIndex = Random.Range(0, deckCards.Count);
                string drawnCard = deckCards[randomCardIndex];

                deckCards.RemoveAt(randomCardIndex);
                GameObject deckCard = deck.transform.GetChild(randomCardIndex).gameObject;

                Debug.Log(deckCards.Count + ": DeckCount");
                deckCard.SetActive(false);

                if (!deckCard.activeSelf)
                {
                    playerHand.Add(drawnCard);

                    //GameObject cardObj = Instantiate(cardPrefab, playerHandTransform);
                    GameObject cardObj = Instantiate(cardPrefab, deck);
                    Card card = cardObj.GetComponent<Card>();

                    StartCoroutine(MoveCard(cardObj, playerHandTransform, 0.45f));
                    //cardObj.transform.DOLocalMove(playerHandTransform.localPosition, 0.35f);
                    //cardObj.transform.SetParent(playerHandTransform);

                    card.CardType = CardType.NormalCard;
                    card.Initialize(drawnCard);

                    if (card.CardType == CardType.NormalCard)
                    {
                        for (int i = 0; i < playerSuperCards.Count; i++)
                        {
                            if (playerSuperCards[i] == card.CardName)
                            {
                                playerSuperCards.RemoveAt(i);
                                playerSuperCardsObj[i].SetActive(false);
                            }

                            if (playerSuperCards.Count == 0)
                            {
                                Debug.Log("Player Win");
                                drawBtn.enabled = false;
                                drawBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Player Win";

                                gamePanel.SetActive(true);
                                gamePanelText.text = "Player Win..!";
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Deck is Empty");
                gamePanel.SetActive(true);
                gamePanelText.text = "Deck is Empty Plese Try Again..!";
            }
        }
        else
        {
            if (deckCards.Count > 0)
            {
                int randomCardIndex = Random.Range(0, deckCards.Count);
                string drawnCard = deckCards[randomCardIndex];

                deckCards.RemoveAt(randomCardIndex);
                GameObject deckCard = deck.transform.GetChild(randomCardIndex).gameObject;
                deckCard.SetActive(false);

                if (!deckCard.activeSelf)
                {
                    opponentHand.Add(drawnCard);
                    Debug.Log(deckCards.Count + ": DeckCount");
                    //GameObject cardObj = Instantiate(cardPrefab, opponentHandTransform);
                    GameObject cardObj = Instantiate(cardPrefab, deck);
                    Card card = cardObj.GetComponent<Card>();

                    StartCoroutine(MoveCard(cardObj, opponentHandTransform, 0.45f));

                    card.CardType = CardType.NormalCard;
                    card.Initialize(drawnCard);
                    deckCards.Remove(drawnCard);

                    if (card.CardType == CardType.NormalCard)
                    {
                        for (int i = 0; i < opponentSuperCards.Count; i++)
                        {
                            if (opponentSuperCards[i] == card.CardName)
                            {
                                opponentSuperCards.RemoveAt(i);
                                opponentSuperCardObj[i].SetActive(false);
                            }

                            if (opponentSuperCards.Count == 0)
                            {
                                Debug.Log("opponent Win");
                                drawBtn.enabled = false;
                                drawBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Opponent Win";
                                gamePanel.SetActive(true);
                                gamePanelText.text = "Opponent Win..!";
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Deck is Empty");
                gamePanel.SetActive(true);
                gamePanelText.text = "Deck is Empty Plese Try Again..!";
            }
        }
        EndTurn();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EndTurn()
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;
            StartCoroutine(PlayerTurn(2));
        }
        else
        {
            isPlayerTurn = true;
            StartCoroutine(PlayerTurn(1));
        }
    }

    private IEnumerator PlayerTurn(int playerNumber)
    {
        yield return null;
    }

    public void PlayCard(string cardName)
    {
        if (gameIsOver)
        {
            return;
        }
    }
}
