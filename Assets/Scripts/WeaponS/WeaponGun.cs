using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gunObject;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float shootForce = 20f;
    [SerializeField] private float fireRate = 0.2f;          // Min seconds between shots
    [SerializeField] private float projectileLifetime = 5f;  // Auto-destroy after N seconds

    private InputAction _shootAction;
    private float _nextFireTime;

    void Awake()
    {
        // Defines the action programmatically — swap for an InputActionAsset if preferred
        _shootAction = new InputAction("Shoot", binding: "<Mouse>/leftButton");
        _shootAction.Enable();
    }

    void OnDestroy()
    {
        _shootAction.Disable();
        _shootAction.Dispose();
    }

    void Update()
    {
        if (_shootAction.WasPressedThisFrame())
        {
            TryShoot();
        }
    }

    void TryShoot()
{
    if (Time.time < _nextFireTime) return;

    // No dispara si la pistola no está activa
    if (gunObject != null && !gunObject.activeInHierarchy) return;

    if (projectilePrefab == null || firePoint == null)
    {
        Debug.LogWarning("[WeaponGun] Missing projectilePrefab or firePoint reference.", this);
        return;
    }

    Shoot();
    _nextFireTime = Time.time + fireRate;
}

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Destroy(projectile, projectileLifetime);  // Prevent memory leak

        if (projectile.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);
        }
    }
}