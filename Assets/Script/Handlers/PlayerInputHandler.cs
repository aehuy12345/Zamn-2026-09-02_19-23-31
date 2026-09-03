using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponHandler weaponHandler;
    [SerializeField] private CharacterAnimationHandler animHandler;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (weaponHandler == null) weaponHandler = GetComponent<WeaponHandler>();
        if (animHandler == null) animHandler = GetComponent<CharacterAnimationHandler>();
    }

    private void Update()
    {
        // 1. Lấy input di chuyển từ bàn phím (WASD / Mũi tên)
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // 2. Xoay vũ khí: Ưu tiên khóa Enemy gần nhất, nếu không có Enemy thì xoay theo hướng di chuyển
        if (weaponHandler != null)
        {
            weaponHandler.AimAtTargetOrDirection(moveInput);
        }

        // 3. Cập nhật Animation di chuyển
        if (animHandler != null)
        {
            animHandler.PlayMove(moveInput.sqrMagnitude > 0.01f);
        }

        // 4. Xử lý Input Tấn công (Bấm chuột trái hoặc phím Space)
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            if (weaponHandler != null)
            {
                weaponHandler.TryAttack();
            }
        }
    }

    private void FixedUpdate()
    {
        // Lấy tốc độ từ CharacterStats hoặc fallback về giá trị mặc định nếu không tìm thấy
        float speed = 3f;
        if (TryGetComponent<CharacterStats>(out var stats) && stats.StatsData != null)
        {
            speed = stats.StatsData.MoveSpeed; // Dùng trực tiếp từ StatsData
        }

        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }
}