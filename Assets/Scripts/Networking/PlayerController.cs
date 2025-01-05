using Fusion;
using UnityEngine;

public class PlayerController : SimulationBehaviour, IPlayerJoined
{
    public int PlayerId { get; private set; }

    public GameObject playerPrefab;  
    public GinGameState gameState;

    public void Initialize(GinGameState state)
    {
        gameState = state;
        Debug.Log("GinGameState initialized in SimulationBehaviour.");
    }
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {

            var spawnedPlayer = Runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
            PlayerId = player.PlayerId;
            Debug.Log($"Local Player with ID {PlayerId} initialized.");

            var networkId = spawnedPlayer.GetComponent<NetworkObject>().Id;
            gameState.AddPlayer(player.PlayerId, networkId);
        }
        else
        {
            Debug.Log($"Player initialized from a DIFFERENT SOURCE!!!!! GOOD JOB!!!!!!!! AND WE DIDNT SPAWN EXTRAS!!!! HOPEFULLY ");
        }
    }
}
