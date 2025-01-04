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
}
