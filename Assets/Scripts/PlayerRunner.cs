using Fusion;
using UnityEngine;

public class PlayerRunner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefabs;
    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("spaw");
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(PlayerSpaw[chooseChatacter], new Vector3(vX, vY, vZ), Quaternion.identity, player,
                (runner, obj) =>
                {
                    // Sau khi spawn xong, bạn có thể setup camera hoặc logic khác
                    var playerSetup = obj.GetComponent<PlayerSetUp>();
                    if (playerSetup != null)
                    {
                        playerSetup.SetUpCamera();
                    }

                });


        }
    }
}




