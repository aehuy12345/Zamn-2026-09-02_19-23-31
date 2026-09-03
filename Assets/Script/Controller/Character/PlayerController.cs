using UnityEngine;
using Unity.Cinemachine;


public class PlayerController : BaseCharacter
{
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // Tự động gán Player làm Target cho Cinemachine Camera khi game bắt đầu
        SetupCameraFollow();
    }

    private void SetupCameraFollow()
    {
        // Cinemachine v3
        var vcam = FindAnyObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.Follow = transform;
        }

        /* 
        // Nếu dùng Cinemachine v2:
        var vcamV2 = FindAnyObjectByType<CinemachineVirtualCamera>();
        if (vcamV2 != null)
        {
            vcamV2.Follow = transform;
        }
        */
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y).normalized;

        HandleFlip(x);

        bool isMoving = moveInput.sqrMagnitude > 0;
        animHandler.PlayMove(isMoving);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleFlip(float horizontalInput)
    {
        if (spriteRenderer == null) return;

        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}