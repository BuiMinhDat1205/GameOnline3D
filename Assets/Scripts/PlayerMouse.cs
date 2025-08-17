using Fusion;
using UnityEngine;

public class PlayerMouse : NetworkBehaviour
{
    public float doNhayChuot = 50f;
    private float xXoayMat = 0f; // Pitch (lên/xuống)
    private float yXoayMat = 0f; // Yaw (trái/phải)

    [Header("Transform References")]
    public Transform camTransform;   // Camera trong prefab
    public Transform headTransform;  // Head (cha của Arms + Gun + FirePoint)

    private void Start()
    {
        if (Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        // Input chuột
        float mouseY = Input.GetAxis("Mouse Y") * doNhayChuot * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * doNhayChuot * Time.deltaTime;

        // Xoay pitch
        xXoayMat -= mouseY;
        xXoayMat = Mathf.Clamp(xXoayMat, -90f, 90f);

        // Xoay yaw
        yXoayMat += mouseX;
        transform.rotation = Quaternion.Euler(0f, yXoayMat, 0f);

        // Xoay head (Arms + Gun + Camera)
        if (headTransform != null)
        {
            headTransform.localRotation = Quaternion.Euler(xXoayMat, 0f, 0f);
        }
    }
}
