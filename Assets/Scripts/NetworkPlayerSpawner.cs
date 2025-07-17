using Fusion;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    public GameObject playerPrefab;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, Object.InputAuthority);
        }
    }
}
