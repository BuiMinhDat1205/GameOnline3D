using UnityEngine;
using Unity.Cinemachine;
using Fusion;

public class CameraSwitch : NetworkBehaviour
{
    [SerializeField] private Camera mainCamera; // Camera vật lý
    [SerializeField][System.Obsolete] private CinemachineVirtualCamera thirdPersonCam; // Virtual camera

    [System.Obsolete]
    void Start()
    {
        // Bật mainCamera, tắt Virtual Camera
        mainCamera.gameObject.SetActive(true);
        thirdPersonCam.gameObject.SetActive(false);
    }

    [System.Obsolete]
    public override void Spawned()
    { 
            thirdPersonCam.gameObject.SetActive(true);
        
    }
    
}
