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

        // Récupérer les voisins de l'ancienne carte
        Card[] neighbours = lastCard.GetNeighbours();
        if (neighbours == null)
            neighbours = new Card[0];

        // Donner les mêmes voisins à la nouvelle carte
        newCard.SetNeighbours(neighbours);

        // Mettre à jour les voisins pour qu'ils pointent vers la nouvelle carte au lieu de l'ancienne
        foreach (Card n in neighbours)
        {
            if (n == null) continue;

            Card[] nNeighbours = n.GetNeighbours();
            if (nNeighbours == null) continue;

            for (int i = 0; i < nNeighbours.Length; i++)
            {
                if (nNeighbours[i] == lastCard)
                {
                    nNeighbours[i] = newCard;
                }
            }

            n.SetNeighbours(nNeighbours);
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