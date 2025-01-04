using UnityEngine;
using Fusion;
using BandCproductions;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        var runner = FindFirstObjectByType<NetworkRunner>();
        runner.ProvideInput = true; // Enable player input handling
    }

    public void SpawnPlayer(NetworkRunner runner, PlayerRef playerRef)
    {
        Debug.Log("Spawn Player : " + runner + playerRef);
        // Spawn the player on the network
        var playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, playerRef);

        // Initialize player-specific data
        var playerController = playerObject.GetComponent<PlayerController>();
        playerController.InitializePlayer(playerRef.PlayerId, $"Player {playerRef.PlayerId}");

        // Add the player to the GinGameState
        var gameState = FindFirstObjectByType<GinGameState>();
        gameState.AddPlayer(playerRef.PlayerId, playerObject.GetComponent<NetworkObject>().Id);
    }
}
