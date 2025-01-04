using Fusion;
using System;

[Serializable]
public struct PlayerHand : INetworkStruct
{
    [Networked, Capacity(11)] // Assuming a max hand size of 10
    public NetworkArray<int> Cards => default;

    // Add a card to the player's hand
    public bool AddCard(int cardId)
    {
        for (int i = 0; i < Cards.Length; i++)
        {
            if (Cards.Get(i) == 0) // Empty slot
            {
                Cards.Set(i, cardId);
                return true; // Successfully added
            }
        }
        return false; // Hand is full
    }

    // Remove a card from the player's hand
    public bool RemoveCard(int cardId)
    {
        for (int i = 0; i < Cards.Length; i++)
        {
            if (Cards.Get(i) == cardId)
            {
                Cards.Set(i, 0); // Remove by setting to 0
                return true; // Successfully removed
            }
        }
        return false; // Card not found
    }

    // Clear the hand
    public void ClearHand()
    {
        for (int i = 0; i < Cards.Length; i++)
        {
            Cards.Set(i, 0); // Clear all slots
        }
    }

    // Get the count of cards in the hand
    public int CardCount()
    {
        int count = 0;
        for (int i = 0; i < Cards.Length; i++)
        {
            if (Cards.Get(i) != 0) count++;
        }
        return count;
    }
}
