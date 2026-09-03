using UnityEngine;

[RequireComponent(typeof(WeaponHandler))]
public class PlayerInputHandler : MonoBehaviour
{
    private WeaponHandler weaponHandler;

    private void Awake()
    {
        weaponHandler = GetComponent<WeaponHandler>();
    }

    private void Update()
    {
        HandleAiming();
        HandleAttackInput();
    }

    private void HandleAiming()
    {
        // Lấy vị trí chuột trong không gian 2D World
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        
        weaponHandler.AimAt(mousePosition);
    }

    private void HandleAttackInput()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0))
        {
            weaponHandler.TryAttack();
        }
    }
}