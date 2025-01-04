using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] private Button startButton;

    private bool gameStarted = false;

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
        PauseGame();

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Start button is not assigned.");
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        startButton.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        Debug.Log("Game Started!");
    }

    public bool IsGameStarted()
    {
        return gameStarted;
    }

    private void PauseGame()
    {
        gameStarted = false;
        Time.timeScale = 0; 
    }
}
