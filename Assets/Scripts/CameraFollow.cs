using Unity;
using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public new CinemachineCamera camera;
    [SerializeField] public CinemachineThirdPersonFollow cam3rd;
    public int soLanBamChuot1 = 0;

    public void AssignCamera(Transform player)
    {
        camera.Follow = player;
        camera.LookAt = player;
    }
    private void Update()
    {
        GocNhinThu3();

        GocNhinCHinhDien();


    }
    public void GocNhinThu3()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && soLanBamChuot1 == 0)
        {
            Debug.Log("BAT1");
            cam3rd.ShoulderOffset = new Vector3(0.8f, 2.90f, -5.61f); // Góc ngắm xa
            soLanBamChuot1 = 1;
        }
        // Nhấn phím 1 khi đã nhấn Ctrl trước đó
        else if (Input.GetKeyDown(KeyCode.Alpha1) && soLanBamChuot1 == 1)
        {
            Debug.Log("TAT1");
            cam3rd.ShoulderOffset = new Vector3(0f, 0.5f, 4.5f); // Góc gần
            soLanBamChuot1 = 0;
        }
    }
    public void GocNhinCHinhDien()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cam3rd.ShoulderOffset = new Vector3(0.8f, 2.02f, 1.17f); // Góc gần

            camera.Lens.FieldOfView = 10.5f;


            Debug.Log("BAT2");
        }
        // Nhấn phím 1 khi đã nhấn Ctrl trước đó
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            if (soLanBamChuot1 != 0)
            {
                cam3rd.ShoulderOffset = new Vector3(0.8f, 2.90f, -5.61f); // Góc ngắm xa
            }
            else { cam3rd.ShoulderOffset = new Vector3(0.8f, 2.02f, 1.17f); }// Góc gần}
            camera.Lens.FieldOfView = 60f;

            Debug.Log("TAT2");
        }
    }
}
