using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    public NetworkRunner runnerPrefab;
    public GameObject playerPrefab;

    private void Start()
    {
        var runner = Instantiate(runnerPrefab);
        runner.ProvideInput = true;
        runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "TestRoom",
            PlayerCount = 2,
            Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
    }
}
