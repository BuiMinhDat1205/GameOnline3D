using Fusion;
using UnityEngine;

public class PlayerRunner : SimulationBehaviour, IPlayerJoined
{
    [Header("Player Setup")]
    public GameObject[] PlayerSpaw; // Danh sách prefab các nhân vật
    public int chooseChatacter = 0;

    [Header("Spawn Position")]
    public float vX = 0f;
    public float vY = 1f;
    public float vZ = 0f;

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
}
