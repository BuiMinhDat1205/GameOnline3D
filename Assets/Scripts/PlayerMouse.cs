using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMouse : NetworkBehaviour
{
    public float doNhayChuot = 50f;
    public float xXoayMat = 0f;
    public float yXoayMat = 0f;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public override void FixedUpdateNetwork()
    {
        Cursor.lockState = CursorLockMode.Locked;
        float yMouse = Input.GetAxisRaw("Mouse Y") * doNhayChuot * Time.deltaTime;
        float xMouse = Input.GetAxisRaw("Mouse X") * doNhayChuot * Time.deltaTime;

        xXoayMat -= yMouse;
        xXoayMat = Mathf.Clamp(xXoayMat, -50, 90);

        yXoayMat += xMouse;
        transform.rotation = Quaternion.Euler(0, yXoayMat, 0);

    }
   
}