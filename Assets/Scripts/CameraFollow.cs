using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public CinemachineCamera camera;
    [SerializeField] public CinemachineThirdPersonFollow cam3rd;
    public int soLanBamChuot1 = 0;

    public void AssignCamera(Transform player)
    {
        camera.Follow = player;
        camera.LookAt = player;


    }
}
