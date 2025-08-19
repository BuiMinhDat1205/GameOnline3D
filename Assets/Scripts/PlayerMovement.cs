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

    [Header("Footstep Settings")]
    public AudioSource footstepAudioSource;
    public AudioClip footstepClip;
    public float footstepInterval = 0.5f; // Time between steps
    private float footstepTimer = 0f;

    // --- Thêm biến quản lý trạng thái không giảm mana ---
    private bool isManaProtected = false;
    private float manaProtectTimer = 0f;
    private float manaProtectDuration = 10f;

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

        // --- Quản lý thời gian trạng thái không giảm mana ---
        if (isManaProtected)
        {
            manaProtectTimer += Runner.DeltaTime;
            if (manaProtectTimer >= manaProtectDuration)
            {
                isManaProtected = false;
                manaProtectTimer = 0f;
            }
        }

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

        // --- Mana ---
        if (isRunning)
        {
            if (!isManaProtected) // Chỉ giảm mana khi không có trạng thái bảo vệ
            {
                mana -= manaDrainRate * Runner.DeltaTime;
                if (mana < 0) mana = 0;
            }
        }
        else
        {
            mana += manaRegenRate * Runner.DeltaTime;
            if (mana > maxMana) mana = maxMana;
        }

        // --- Footstep Sound ---
        if (_controller.isGrounded && isMoving && !_isCrouching)
        {
            footstepTimer += Runner.DeltaTime;
            if (footstepTimer >= footstepInterval)
            {
                if (footstepAudioSource != null && footstepClip != null)
                {
                    footstepAudioSource.pitch = Random.Range(0.95f, 1.05f); // Optional variation
                    footstepAudioSource.PlayOneShot(footstepClip);
                }
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
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

    // Hàm public để kích hoạt trạng thái không giảm mana 10 giây
    public void ActivateManaProtection()
    {
        isManaProtected = true;
        manaProtectTimer = 0f;
    }
    // Add this method to the PlayerMovement class
    public void AddMana(float amount)
    {
        mana += amount;
        if (mana > maxMana)
            mana = maxMana;
    }
}

