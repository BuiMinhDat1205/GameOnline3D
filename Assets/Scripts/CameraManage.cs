using UnityEngine;
using Unity.Cinemachine;
using Fusion;

public class CameraSwitch : NetworkBehaviour
{
    [SerializeField] private Camera mainCamera; // Camera vật lý
    [SerializeField] private CinemachineCamera thirdPersonCam; // Virtual camera

    void Start()
    {
        // Bật mainCamera, tắt Virtual Camera
        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (thirdPersonCam != null) thirdPersonCam.gameObject.SetActive(false);
    }

    public override void Spawned()
    {
        if (thirdPersonCam != null)
            thirdPersonCam.gameObject.SetActive(true);
    }
}
