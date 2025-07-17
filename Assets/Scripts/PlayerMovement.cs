using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController controller;
    public float speed = 5f;

    private void Awake()
    {
        // Gán controller nếu chưa gán trong Inspector
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Chỉ player local (có quyền điều khiển) mới được xử lý input
        if (!Object.HasStateAuthority) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = Vector3.right * x + Vector3.forward * z;

        controller.Move(move * speed * Runner.DeltaTime); // Dùng Runner.DeltaTime cho đúng network tick
    }
}
