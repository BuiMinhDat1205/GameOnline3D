using Unity;
using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{
    public new CinemachineCamera camera;
    [SerializeField] public CinemachineThirdPersonFollow cam3rd;
    public int soLanBamChuot1 = 0;

    private void Update()
    {
        GocNhinThu3();
        GocNhinCHinhDien();
    }

    public void AssignCamera(Transform head)
    {
        camera.Follow = head;
        camera.LookAt = head;
    }

    public void GocNhinThu3()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && soLanBamChuot1 == 0)
        {
            cam3rd.ShoulderOffset = new Vector3(0f, 1.45f, 0.7f);
            soLanBamChuot1 = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && soLanBamChuot1 == 1)
        {
            cam3rd.ShoulderOffset = new Vector3(0f, 3f, -4.5f);
            soLanBamChuot1 = 0;
        }
    }

    public void GocNhinCHinhDien()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cam3rd.ShoulderOffset = new Vector3(0f, 1.45f, 0.7f);
            camera.Lens.FieldOfView = 10.5f;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            if (soLanBamChuot1 != 0)
                cam3rd.ShoulderOffset = new Vector3(0f, 1.45f, 0.7f);
            else
                cam3rd.ShoulderOffset = new Vector3(0f, 3f, -4.5f);

            camera.Lens.FieldOfView = 60f;
        }
    }
}
