using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitch : MonoBehaviour
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            bool isMainActive = mainCamera.gameObject.activeSelf;

            mainCamera.gameObject.SetActive(!isMainActive);
            thirdPersonCam.gameObject.SetActive(isMainActive);
        }
    }
}
