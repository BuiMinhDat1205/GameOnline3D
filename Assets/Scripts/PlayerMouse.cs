using Fusion;
using UnityEngine;

public class PlayerMouse : NetworkBehaviour
{
    public float doNhayChuot = 50f;
    private float xXoayMat = 0f; // Trục dọc (lên xuống)
    private float yXoayMat = 0f; // Trục ngang (xoay trái/phải)

    public Transform camTransform;   // Gắn Camera (hoặc Arms)
    public Transform headTransform;  // Gắn phần đầu/tay để xoay lên xuống

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void FixedUpdateNetwork()
    {
        // Input chuột
        float mouseY = Input.GetAxis("Mouse Y") * doNhayChuot * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * doNhayChuot * Time.deltaTime;

        // Xử lý xoay dọc (chỉ ảnh hưởng đầu, tay, camera)
        xXoayMat -= mouseY;
        xXoayMat = Mathf.Clamp(xXoayMat, -90f, 90f);

        // Xử lý xoay ngang (xoay toàn bộ player)
        yXoayMat += mouseX;
        transform.rotation = Quaternion.Euler(0f, yXoayMat, 0f);

        // Xoay đầu (tức phần chứa camera và tay)
        if (headTransform != null)
        {
            headTransform.localRotation = Quaternion.Euler(xXoayMat, 0f, 0f);
        }
    }
}
