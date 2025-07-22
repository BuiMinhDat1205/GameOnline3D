using Fusion;
using UnityEngine;

public class PlayerRunner1 : SimulationBehaviour, IPlayerJoined
{
    public GameObject chooserMode;
    public GameObject startGame;

    [Header("Player Setup")]
    public GameObject[] PlayerSpaw; // Danh sách prefab các nhân vật
    public int chooseChatacter = 0;

    [Header("Spawn Position")]
    public float vX = 0f;
    public float vY = 1f;
    public float vZ = 0f;

    private void Start()
    {
        chooserMode.SetActive(true);
        startGame.SetActive(false);
    }
    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("Spawn Player");

        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(
                PlayerSpaw[chooseChatacter],
                new Vector3(vX, vY, vZ),
                Quaternion.identity,
                player,
                (runner, obj) =>
                {
                    // Sau khi spawn thành công
                    var playerSetup = obj.GetComponent<PlayerSetUp>();
                    if (playerSetup != null)
                    {
                        playerSetup.SetUpCamera(); // Gắn camera follow player
                    }
                                          
                });
  

        }
    }

    public void ChooseHuman()
    {
        chooseChatacter = 0;
        //Destroy(chooserMode);
        chooserMode.SetActive(false);
        startGame.SetActive(true);
    }
    public void ChooseHulk()
    {
        chooseChatacter = 1;
        //Destroy(chooserMode);
        chooserMode.SetActive(false);
        startGame.SetActive(true);
    }
}
