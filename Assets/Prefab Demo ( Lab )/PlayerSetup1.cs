using Fusion;
using UnityEngine;

public class PlayerSetUp1 : NetworkBehaviour
{
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            CameraFollow1 camera = FindObjectOfType<CameraFollow1>();
            if (camera != null)
            {
                camera.AssignCamera1(transform);
            }
        }
    }
}
