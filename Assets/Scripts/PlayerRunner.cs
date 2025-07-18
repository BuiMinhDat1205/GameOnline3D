using Fusion;
using UnityEngine;

public class PlayerRunner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefabs;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            // Spawn player có gán quyền điều khiển đúng (inputAuthority)
            Runner.Spawn(PlayerPrefabs, new Vector3(0, 1, 0), Quaternion.identity, player);
        }
    }
}
