using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum CardType
{
    None,
    NormalCard,
    SuperCard
}

public class Card : MonoBehaviour
{
    public CardType CardType = CardType.None;

    public string CardName; //{ get; private set; }

    public TMP_Text cardNameText;
    public Image[] cardImage;

    public void Initialize(string cardName)
    {
        CardName = cardName;
        cardNameText.text = cardName;

        if(CardType == CardType.NormalCard)
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
