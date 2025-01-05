using Fusion;
using System.Collections.Generic;
using System.Collections;
using BandCproductions;
using UnityEngine;
using System;
using System.Linq;
using Fusion.Sockets;

public class GinGameState : NetworkBehaviour
{
    [Networked]
    [Capacity(52)]
    private NetworkLinkedList<int> DeckState { get; }

    [Networked]
    [Capacity(52)]
    private NetworkLinkedList<int> DiscardState { get; }

    [Networked]
    [Capacity(2)]
    public NetworkDictionary<int, NetworkId> PlayerStates { get; }

    [Networked]
    [Capacity(2)]
    public NetworkDictionary<int, PlayerHand> PlayerHands { get; }

    [Networked]
    [Capacity(2)]
    public NetworkDictionary<int, bool> PlayerReadyStates { get; }




    public static GinGameState Instance {get; private set;}
    public GameController gameController; 


    public override void Spawned()
    {
        Instance = this;

        var simBehaviour = Runner.GetBehaviour<PlayerController>();
        simBehaviour.Initialize(this);
        /*
        if (HasStateAuthority)
        {
            InitializeDeck();
            StartCoroutine(WaitForPlayersThenDeal(10));
        }*/
        //PlayerStates.Clear();
        //DeckState.Clear();
        //DiscardState.Clear();
        Debug.Log("Spawned.");
    }

    public void AddPlayer(int playerId, NetworkId networkId)
    {
        if (HasStateAuthority && !PlayerStates.ContainsKey(playerId))
        {
            PlayerStates.Add(playerId, networkId);
            Debug.Log($"Player {playerId} added with NetworkId {networkId}.");
        }
    }
    /*
    public void AddCardToDeckState(int cardId)
    {
        DeckState.Add(cardId);
        Debug.Log("Card ID: " + cardId + " has been added to the deck state. Deckstate count: " + DeckState.Count);
    }
    public void AddCardToDiscardState(int cardId)
    {
        DiscardState.Add(cardId);
        Debug.Log("Card ID: " + cardId + " has been added to the discard state. discardstate count: " + DiscardState.Count);
    }

    public void AddCardToHandState(int cardId, int playerId)
    {
        DiscardState.Add(cardId);
        Debug.Log("Card ID: " + cardId + " has been added to the discard state. discardstate count: " + DiscardState.Count);
    }*/

    public void InitializeDeck()
    {
        if (!HasStateAuthority) return;

        List<int> deck = new List<int>();
        for (int i = 1; i <= 52; i++) // Example card IDs: 1 to 52
        {
            deck.Add(i);
        }

        // Shuffle the deck
        System.Random rng = new System.Random();
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = rng.Next(i + 1);
            int temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        // Add to DeckState
        DeckState.Clear();
        foreach (var cardId in deck)
        {
            Debug.Log(cardId);
            DeckState.Add(cardId);
            Debug.Log(DeckState.Count);
        }

        Debug.Log("Deck initialized and shuffled.");
    }

    public void SetPlayerReadyState(bool isReady)
    {
        if (!HasStateAuthority) return;

        var localPlayerId = Runner.LocalPlayer.PlayerId;

        if (PlayerReadyStates.ContainsKey(localPlayerId))
        {
            PlayerReadyStates.Set(localPlayerId, isReady);
        }
        else
        {
            PlayerReadyStates.Add(localPlayerId, isReady);
        }

        Debug.Log($"Player {localPlayerId} is now {(isReady ? "ready" : "not ready")}.");
        CheckAllPlayersReady();
    }

    private void CheckAllPlayersReady()
    {
        if (!HasStateAuthority) return;

        foreach (var readyState in PlayerReadyStates)
        {
            if (!readyState.Value) // If any player is not ready
            {
                Debug.Log("Not all players are ready yet.");
                return;
            }
        }

        Debug.Log("All players are ready! Starting the game...");
        gameController.SetGameStarted();
        InitializeDeck();
        StartCoroutine(WaitForPlayersThenDeal(10));
    }

    private IEnumerator WaitForPlayersThenDeal(int cardsPerPlayer)
    {
        Debug.Log("Waiting for players to join...");

        // Wait until at least one player is present
        yield return new WaitUntil(() => Runner.ActivePlayers.Count() > 1);

        PlayerHands.Clear();
        foreach (var playerRef in Runner.ActivePlayers)
        {
            if (!PlayerHands.ContainsKey(playerRef.PlayerId))
            {
                PlayerHands.Add(playerRef.PlayerId, new PlayerHand());
            }
        }

        Debug.Log("Players joined. Dealing cards...");
        DealCards(cardsPerPlayer);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} joined.");

        if (!PlayerHands.ContainsKey(player.PlayerId))
        {
            PlayerHands.Add(player.PlayerId, new PlayerHand());
            DealCards(10);
        }
    }
    public void DealCards(int cardsPerPlayer)
    {
        if (!HasStateAuthority) return;

        foreach (var player in PlayerStates)
        {
            if (PlayerHands.ContainsKey(player.Key)) // Check if the player has a hand
            {
                var hand = PlayerHands.Get(player.Key); // Retrieve the player's hand

                for (int i = 0; i < cardsPerPlayer; i++)
                {
                    if (DeckState.Count == 0) break;

                    int cardId = DeckState.Get(0);
                    DeckState.Remove(cardId);

                    if (!hand.AddCard(cardId)) // Add card to the hand
                    {
                        Debug.LogWarning($"Player {player.Key}'s hand is full!");
                    }
                    else
                    {
                        Debug.Log($"Dealt card {cardId} to Player {player.Key}.");
                    }
                }

                PlayerHands.Set(player.Key, hand); // Update the hand in the dictionary
                Debug.Log(hand.CardCount());
            }
            else
            {
                Debug.LogWarning($"Player {player.Key} does not have a hand initialized.");
            }
        }
    }

    public void AddCardToPlayerHand(int playerId, int cardId)
    {
        if (!HasStateAuthority) return;

        if (PlayerHands.ContainsKey(playerId)) // Check if the player has a hand
        {
            var hand = PlayerHands.Get(playerId); // Retrieve the player's hand

            if (hand.AddCard(cardId)) // Add the card to the hand
            {
                PlayerHands.Set(playerId, hand); // Update the hand in the dictionary
                Debug.Log($"Added card {cardId} to Player {playerId}'s hand.");
            }
            else
            {
                Debug.LogWarning($"Player {playerId}'s hand is full!");
            }
        }
        else
        {
            Debug.LogWarning($"Player {playerId} does not have a hand initialized.");
        }
    }

    public void RemoveCardFromPlayerHand(int playerId, int cardId)
    {
        if (!HasStateAuthority) return;

        if (PlayerHands.ContainsKey(playerId)) // Check if the player has a hand
        {
            var hand = PlayerHands.Get(playerId); // Retrieve the player's hand

            if (hand.RemoveCard(cardId)) // Remove the card from the hand
            {
                PlayerHands.Set(playerId, hand); // Update the hand in the dictionary
                Debug.Log($"Removed card {cardId} from Player {playerId}'s hand.");
            }
            else
            {
                Debug.LogWarning($"Card {cardId} not found in Player {playerId}'s hand.");
            }
        }
        else
        {
            Debug.LogWarning($"Player {playerId} does not have a hand initialized.");
        }
    }

}