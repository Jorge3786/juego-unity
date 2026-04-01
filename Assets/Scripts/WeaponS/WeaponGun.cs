using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform  firePoint;
    [SerializeField] private Transform  gunVisual;
    [SerializeField] private Transform  rayPoint;

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

    private Coroutine _recoilCoroutine;
    private Coroutine _rayCoroutine;

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

        // IMPORTANTE: evita que armas desactivadas disparen
        if (!gameObject.activeInHierarchy) return;

        if (visualData == null)
        {
            Debug.LogWarning("[WeaponGun] Missing VisualData.", this);
            return;
        }

        Shoot();
        _nextFireTime = Time.time + fireRate;
    }

    void Shoot()
    {
        if (visualData.fireMode == FireMode.Raycast)
            ShootRaycast();
        else
            ShootProjectile();

        // Recoil
        if (gunVisual != null)
        {
            if (_recoilCoroutine != null)
                StopCoroutine(_recoilCoroutine);

            _recoilCoroutine = StartCoroutine(RecoilAnimation());
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("[WeaponGun] Missing projectilePrefab or firePoint.", this);
            return;
        }

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
            rb.useGravity = visualData.hasBulletDrop;
            rb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);
        }
    }

    void ShootRaycast()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        float range   = visualData.raycastRange;
        LayerMask mask = visualData.raycastMask;

        if (Physics.Raycast(ray, out RaycastHit hit, range, mask))
        {
            // Spawn del "impacto" (tu prefab del bastón)
            if (projectilePrefab != null)
            {
                GameObject projectile = Instantiate(
                    projectilePrefab,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );

                Destroy(projectile, projectileLifetime);
            }

            // Rayo visual
            if (rayPoint != null)
            {
                if (_rayCoroutine != null)
                    StopCoroutine(_rayCoroutine);

                _rayCoroutine = StartCoroutine(RayVisual(rayPoint.position, hit.point));
            }
        }
    }

    IEnumerator RecoilAnimation()
    {
        Vector3 recoilPosition = _originalPosition - Vector3.forward * visualData.recoilDistance;
        Quaternion recoilRotation = _originalRotation * Quaternion.Euler(-visualData.recoilRotation, 0f, 0f);

        float t = 0f;

        // Retroceso
        while (t < 1f)
        {
            t += Time.deltaTime * visualData.recoilSpeed;
            gunVisual.localPosition = Vector3.Lerp(_originalPosition, recoilPosition, t);
            gunVisual.localRotation = Quaternion.Lerp(_originalRotation, recoilRotation, t);
            yield return null;
        }

        // Vuelta
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

    IEnumerator RayVisual(Vector3 from, Vector3 to)
    {
        GameObject rayGo = new GameObject("RayVisual");
        LineRenderer lr = rayGo.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        lr.useWorldSpace = true;

        float width = visualData.rayWidth;
        lr.startWidth = width;
        lr.endWidth = width;

        if (visualData.trailMaterial != null)
            lr.material = visualData.trailMaterial;
        else
            lr.material = new Material(Shader.Find("Particles/Standard Unlit"));

        Color color = visualData.trailColor;

        float duration = visualData.rayFadeDuration;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float alpha = Mathf.Lerp(1f, 0f, t);

            lr.startColor = new Color(color.r, color.g, color.b, alpha);
            lr.endColor   = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }

        Destroy(rayGo);
    }
}