using Fusion;
using UnityEngine;

public class PlayerController : SimulationBehaviour, IPlayerJoined
{
    public int PlayerId { get; private set; }

    public GameObject playerPrefab;  

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
            PlayerId = player.PlayerId;
            Debug.Log($"Player with ID {PlayerId} initialized.");
        }
        else
        {
            Debug.Log($"Player with ID {PlayerId} initialized from a DIFFERENT SOURCE!!!!! GOOD JOB!!!!!!!! AND WE DIDNT SPAWN EXTRAS!!!! HOPEFULLY ");
        }
    }
}
