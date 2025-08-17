using Fusion;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour
{
    [Header("References")]
    public Transform headTransform;              // Gán Head1 vào đây
    public CameraFollow playerCameraFollow;      // Gán Third Person Aim Camera vào đây

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            playerCameraFollow.gameObject.SetActive(true);

            // Gán follow/lookAt cho Cinemachine vào head
            playerCameraFollow.AssignCamera(headTransform);
        }
        else
        {
            // Tắt camera cho player không phải local
            playerCameraFollow.gameObject.SetActive(false);
        }
    }
}
