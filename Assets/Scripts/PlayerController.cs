using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Networked] public int PlayerId { get; private set; }
    [Networked] public string PlayerName { get; private set; }

    public void InitializePlayer(int id, string name)
    {
        if (HasStateAuthority)
        {
            PlayerId = id;
            PlayerName = name;
            Debug.Log($"Player {name} with ID {id} initialized.");
        }
    }
}
