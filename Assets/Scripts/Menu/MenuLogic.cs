using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.SceneManagement;
namespace BandCproductions
{
    public class MenuLogic : MonoBehaviour
    {
        [SerializeField] private NetworkRunner _networkRunnerPrefab;

        [SerializeField] private TMP_InputField _nickName;

        // The Placeholder Text is not accessible through the TMP_InputField component so need a direct reference
        [SerializeField] private TextMeshProUGUI _nickNamePlaceholder;

        [SerializeField] private TMP_InputField _roomName;
        [SerializeField] private string _gameScenePath;

        private NetworkRunner _runnerInstance;

        void Start()
        {
            Debug.Log($"_nickName: {_nickName}");
            Debug.Log($"_nickNamePlaceholder: {_nickNamePlaceholder}");
            Debug.Log($"_roomName: {_roomName}");

            if (_nickName == null || _nickNamePlaceholder == null || _roomName == null)
            {
                Debug.LogError("One or more references are null. Ensure the fields are properly assigned in the Inspector.");
            }
        }

        public void StartShared()
        {
            Debug.Log($"nickname :{_nickName}, _nickNamePlaceholder: {_nickNamePlaceholder}, _roomName: {_roomName}, _gameScenePath: {_gameScenePath}");
            SetPlayerData();
            StartGame(GameMode.Shared, _roomName.text, _gameScenePath);
        }

        private void SetPlayerData()
        {
            if (string.IsNullOrWhiteSpace(_nickName.text))
            {
                LocalPlayerData.NickName = _nickNamePlaceholder.text;
            }
            else
            {
                LocalPlayerData.NickName = _nickName.text;
            }
        }

        private async void StartGame(GameMode mode, string roomName, string sceneName)
        {
            _runnerInstance = FindFirstObjectByType<NetworkRunner>();
            if (_runnerInstance == null)
            {
                _runnerInstance = Instantiate(_networkRunnerPrefab);
            }

            // Let the Fusion Runner know that we will be providing user input
            _runnerInstance.ProvideInput = true;

            var startGameArgs = new StartGameArgs()
            {
                GameMode = mode,
                SessionName = roomName,
                Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(_gameScenePath)),
                ObjectProvider = _runnerInstance.GetComponent<NetworkObjectPoolDefault>(),
            };

            // GameMode.Host = Start a session with a specific name
            // GameMode.Client = Join a session with a specific name
            await _runnerInstance.StartGame(startGameArgs);

            if (_runnerInstance.IsServer)
            {
                _runnerInstance.LoadScene(sceneName);
            }
        }
    }
}