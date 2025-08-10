using Fusion;
using UnityEngine;
using UnityEngine.UI; // Để dùng Slider

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;
    private bool _isCrouching; // Thêm biến ngồi

    private CharacterController _controller;
    private Animator _anim; // Animator

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float crouchSpeed = 1f; // Tốc độ khi ngồi
    private float currentSpeed;

    [Header("Jump")]
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    [Header("Mana Settings")]
    [Networked] public float mana { get; set; }
    public float maxMana = 100f;
    public float manaDrainRate = 20f;   // giảm mana/giây khi chạy
    public float manaRegenRate = 10f;   // hồi mana/giây khi không chạy
    public Slider manaSlider;           // Thanh mana UI

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _anim = GetComponentInChildren<Animator>(); // lấy Animator từ con (nếu có)
    }

    public override void Spawned()
    {
        mana = maxMana;

        // Ẩn thanh mana của player khác
        if (!Object.HasInputAuthority && manaSlider != null)
            manaSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!Object.HasInputAuthority) return;

        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }

        // Bật/tắt crouch khi bấm Ctrl
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _isCrouching = !_isCrouching;
        }

        // Hiển thị thanh mana theo phần trăm
        if (manaSlider != null)
        {
            manaSlider.minValue = 0;
            manaSlider.maxValue = 1; // dùng tỉ lệ
            manaSlider.value = mana / maxMana;
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

        bool isMoving = (h != 0 || v != 0);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && mana > 0 && isMoving && !_isCrouching;

        // Tốc độ di chuyển
        if (_isCrouching)
            currentSpeed = crouchSpeed;
        else
            currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Di chuyển
        Vector3 move = (transform.right * h + transform.forward * v).normalized * currentSpeed * Runner.DeltaTime;

        // Gravity + Jump
        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded && !_isCrouching) // Không nhảy khi đang ngồi
        {
            _velocity.y += JumpForce;
            if (_anim != null)
                _anim.SetBool("IsJumping", true);
        }

        _controller.Move(move + _velocity * Runner.DeltaTime);

        // Mana
        if (isRunning)
        {
            mana -= manaDrainRate * Runner.DeltaTime;
            if (mana < 0) mana = 0;
        }
        else
        {
            mana += manaRegenRate * Runner.DeltaTime;
            if (mana > maxMana) mana = maxMana;
        }

        // Animation
        if (_anim != null)
        {
            float moveAmount = new Vector3(h, 0, v).magnitude; // 0 -> 1
            float speedValue = isRunning ? moveAmount : moveAmount * 0.5f;
            _anim.SetFloat("Speed", speedValue, 0.1f, Time.deltaTime);

            _anim.SetBool("IsCrouching", _isCrouching); // Set crouch
            if (_controller.isGrounded && !_jumpPressed)
                _anim.SetBool("IsJumping", false);
        }

        _jumpPressed = false;
    }
}
