using UnityEngine;
using ToolBox.Utils;

public class SSO_CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string cardDescription;
    public Card[] cardPrefabs;

    public Sprite cardUIIcon;

    public SSO_CardsAvailable cardsAvailable;

    public virtual void ApplyEffectToNeighbour(Card card, Transform cardsContent) { }

    protected Card ReplaceCards(Card lastCard, SSO_CardData newData, Transform cardsContent)
    {
        // Instancier la nouvelle carte
        Card newCard = Instantiate(newData.cardPrefabs.GetRandom(), cardsContent);
        newCard.Setup(newData);

        // Copier la transform
        newCard.transform.position = lastCard.transform.position;
        newCard.transform.rotation = lastCard.transform.rotation;

        Card[] neighbours = lastCard.GetNeighbours();
        if (neighbours == null)
            neighbours = new Card[4];

        newCard.SetNeighbours(neighbours);

        // mettre à jour les voisins
        for (int i = 0; i < 4; i++)
        {
            Card n = neighbours[i];
            if (n == null) continue;

            Card[] nNeigh = n.GetNeighbours();
            int opposite = (i + 2) % 4;

            nNeigh[opposite] = newCard;
            n.SetNeighbours(nNeigh);
        }

        // Mettre à jour le dictionnaire dans le CardPlacementManager
        if (CardPlacementManager.Instance != null)
        {
            CardPlacementManager.Instance.OnCardReplaced(lastCard, newCard);
        }

        // Détruire l'ancienne carte une fois que toutes les références pointent vers la nouvelle
        Destroy(lastCard.gameObject);

        return newCard;
    }
}