using UnityEngine;
using Fusion;

public class KoWNetworkManager : MonoBehaviour
{
    private NetworkRunner _runner;

    async void Start()
    {
        // Add the NetworkRunner component if it's not already there
        _runner = GetComponent<NetworkRunner>();
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
        }

        // Start the network runner
        await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared, // Shared mode for Quantum
            SessionName = "GinGameSession", // Session name (can be anything)
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>() // Handles scene synchronization
        });

        Debug.Log("Network Runner started in Shared mode.");
    }
}
