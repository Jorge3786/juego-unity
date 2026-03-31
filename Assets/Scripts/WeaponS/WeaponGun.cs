using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gunObject;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform  firePoint;
    [SerializeField] private Transform  gunVisual;

    [Header("Settings")]
    [SerializeField] private float shootForce        = 20f;
    [SerializeField] private float fireRate          = 0.2f;
    [SerializeField] private float projectileLifetime = 5f;

    [Header("Visual")]
    [SerializeField] private WeaponVisualData visualData;

    private InputAction _shootAction;
    private float       _nextFireTime;

    private Vector3    _originalPosition;
    private Quaternion _originalRotation;
    private Coroutine  _recoilCoroutine;

    void Awake()
    {
        _shootAction = new InputAction("Shoot", binding: "<Mouse>/leftButton");
        _shootAction.Enable();
    }

    void Start()
    {
        if (gunVisual != null)
        {
            _originalPosition = gunVisual.localPosition;
            _originalRotation = gunVisual.localRotation;
        }
    }

    void OnDestroy()
    {
        _shootAction.Disable();
        _shootAction.Dispose();
    }

    void Update()
    {
        if (_shootAction.WasPressedThisFrame())
            TryShoot();
    }

    void TryShoot()
        {
            if (Time.time < _nextFireTime) return;

            if (!gameObject.activeInHierarchy) return; // <-- comprueba el propio GameObject

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
        Destroy(projectile, projectileLifetime);

        Collider[] playerColliders = GetComponentsInParent<Collider>();
        Collider projectileCollider = projectile.GetComponent<Collider>();

        if (projectileCollider != null)
        {
            foreach (Collider col in playerColliders)
                Physics.IgnoreCollision(projectileCollider, col);
        }

        if (projectile.TryGetComponent(out Rigidbody rb))
        {
            rb.useGravity = visualData != null && visualData.hasBulletDrop;
            rb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);
        }

        if (gunVisual != null && visualData != null)
        {
            if (_recoilCoroutine != null)
                StopCoroutine(_recoilCoroutine);
            _recoilCoroutine = StartCoroutine(RecoilAnimation());
        }
    }

    IEnumerator RecoilAnimation()
    {
        Vector3    recoilPosition = _originalPosition - Vector3.forward * visualData.recoilDistance;
        Quaternion recoilRotation = _originalRotation * Quaternion.Euler(-visualData.recoilRotation, 0f, 0f);

        // Fase 1 — retroceso
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * visualData.recoilSpeed;
            gunVisual.localPosition = Vector3.Lerp(_originalPosition, recoilPosition, t);
            gunVisual.localRotation = Quaternion.Lerp(_originalRotation, recoilRotation, t);
            yield return null;
        }

        // Fase 2 — vuelta a la posición original
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * visualData.returnSpeed;
            gunVisual.localPosition = Vector3.Lerp(recoilPosition, _originalPosition, t);
            gunVisual.localRotation = Quaternion.Lerp(recoilRotation, _originalRotation, t);
            yield return null;
        }

        gunVisual.localPosition = _originalPosition;
        gunVisual.localRotation = _originalRotation;
    }
}