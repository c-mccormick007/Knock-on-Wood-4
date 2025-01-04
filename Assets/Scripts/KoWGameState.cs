using Fusion;
using System.Collections.Generic;
using BandCproductions;
using UnityEngine;
using System;
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

    public Player player;


    public override void Spawned()
    {
        PlayerStates.Clear();
        DeckState.Clear();
        DiscardState.Clear();
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
    }


}