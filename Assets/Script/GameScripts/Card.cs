using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public enum CardType
{
    None,
    NormalCard,
    SuperCard
}

public class Card : MonoBehaviourPunCallbacks
{
    public CardType CardType = CardType.None;

    public string CardName; //{ get; private set; }

    public TMP_Text cardNameText;
    public Image[] cardImage;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");

    }
    public void Initialize(string cardName)
    {
        CardName = cardName;

        if (photonView.IsMine)
        {
            cardNameText.gameObject.SetActive(true);
        }
        else
        {
            cardNameText.gameObject.SetActive(false);
        }

        cardNameText.text = cardName;

        if (photonView.IsMine)
        {
            if (CardType == CardType.NormalCard)
            {
                gameObject.GetComponent<Image>().sprite = cardImage[0].sprite;
            }
            else
            {
                gameObject.GetComponent<Image>().sprite = cardImage[1].sprite;
            }
        }
        else
        {
            gameObject.GetComponent<Image>().color = Color.white;
        }

        if (CardType == CardType.NormalCard)
        {
            gameObject.GetComponent<Image>().sprite = cardImage[0].sprite;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = cardImage[1].sprite;
        }
    }

    public void Offcard(string cardName)
    {
        CardName = cardName;
        cardNameText.text = cardName;

        gameObject.SetActive(false);
    }

    public void PlayCard()
    {
        GameManager.instance.PlayCard(CardName);
    }


    /*public bool IsSelected { get; private set; }
    public void OnSelected(bool selected)
    {
        IsSelected = selected;

        if (selected)
        {
            transform.position = (Vector2)transform.position + Vector2.up * 0.3f;
        }
        else
        {
            transform.position = (Vector2)transform.position - Vector2.up * 0.3f;
        }
    }*/

    /*public void OnPointerClick(PointerEventData eventData)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Card card = gameObject.GetComponent<Card>();
            
            if (GameManager.instance.selectedCard == card)
            {
                card.OnSelected(!card.IsSelected);
            }
            else
            {
                // Deselect the previously selected card (if any)
                if (GameManager.instance.selectedCard != null)
                {
                    GameManager.instance.selectedCard.OnSelected(false);
                }

                // Select the new card
                GameManager.instance.selectedCard = card;
                card.OnSelected(true);
            }
        }
    }*/
}
