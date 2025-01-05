using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] private Button readyUpButton;
    [SerializeField] private TextMeshProUGUI playerStatusText;

    private bool isReady = false;
    private bool isGameStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        readyUpButton.onClick.AddListener(OnReadyUpClicked);
    }

    private void OnReadyUpClicked()
    {
        isReady = !isReady; // Toggle readiness state
        readyUpButton.GetComponentInChildren<TextMeshProUGUI>().text = isReady ? "Cancel Ready" : "Ready Up";

        // Notify GinGameState of player's readiness
        GinGameState.Instance.SetPlayerReadyState(isReady);
    }

    public void UpdatePlayerStatus(string status)
    {
        playerStatusText.text = status;
    }
    public void SetGameStarted()
    {
        isGameStarted = true;
    }
    public bool IsGameStarted()
    {
        return isGameStarted;
    }
}
