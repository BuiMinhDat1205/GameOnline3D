using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;

    private CharacterController _controller;

    public float PlayerSpeed = 2f;

    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        // Lấy input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Di chuyển theo local space (hướng của nhân vật)
        Vector3 move = (transform.right * h + transform.forward * v).normalized * PlayerSpeed * Runner.DeltaTime;

        // Gravity + Jump
        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += JumpForce;
        }

        // Thực hiện di chuyển
        _controller.Move(move + _velocity * Runner.DeltaTime);

        // Không thay đổi hướng nhân vật
        // Nếu muốn xoay theo trục Y bằng input, có thể thêm đoạn xoay bên ngoài

        _jumpPressed = false;
    }
}