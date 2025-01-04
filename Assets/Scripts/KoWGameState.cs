using Fusion;
using System.Collections.Generic;
using BandCproductions;
using UnityEngine;
using System;
using Fusion.Sockets;

public class GinGameState : NetworkBehaviour, INetworkRunnerCallbacks
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
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        Debug.Log($"Player joined: {playerRef.PlayerId}");
        player.SpawnPlayer(runner, playerRef);
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        Debug.Log($"Player left: {playerRef.PlayerId}");
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


    #region Unused Callbacks
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server.");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"Connection failed to {remoteAddress}. Reason: {reason}");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("Connect request received.");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("Custom authentication response received.");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected from server. Reason: {reason}");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Host migration started.");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Poll for user input here if needed.
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log($"Input missing for player {player.PlayerId}");
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player)
    {
        Debug.Log($"Object {networkObject.name} entered AOI for player {player.PlayerId}");
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player)
    {
        Debug.Log($"Object {networkObject.name} exited AOI for player {player.PlayerId}");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        Debug.Log($"Reliable data received for player {player.PlayerId}");
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        Debug.Log($"Reliable data progress: {progress * 100}% for player {player.PlayerId}");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene load done.");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene load started.");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Session list updated.");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"Runner shutdown. Reason: {shutdownReason}");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("User simulation message received.");
    }

    #endregion
}

