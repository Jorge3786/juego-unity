using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponGun : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform firePoint;         // Donde se instancia el proyectil
    public float shootForce = 20f;      // Fuerza del proyectil si tiene Rigidbody

    void Update()
    {
        // Verifica que el mouse esté disponible
        if (Mouse.current == null) return;

        // Botón izquierdo del mouse para disparar
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Aplica fuerza si tiene Rigidbody
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);
        }

        // Si quieres usar partículas en vez de Rigidbody, simplemente
        // asegúrate de que projectilePrefab tenga un ParticleSystem
        // y no necesita Rigidbody ni fuerza.
    }
}