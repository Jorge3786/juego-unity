using UnityEngine;

public class ProjectileBullet : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private TrailRenderer trail;

    void Awake()
    {
        // Auto-busca el TrailRenderer si no está asignado
        if (trail == null)
            trail = GetComponentInChildren<TrailRenderer>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Spawn de impacto antes de destruir
        SpawnImpact(collision.contacts[0].point, collision.contacts[0].normal);
        Destroy(gameObject);
    }

    void SpawnImpact(Vector3 point, Vector3 normal)
    {
        // Detach del trail para que termine su animación sola
        if (trail != null)
        {
            trail.transform.SetParent(null);
            Destroy(trail.gameObject, trail.time + 0.1f);
        }

        // Pequeña partícula de impacto con ParticleSystem
        var go = new GameObject("BulletImpact");
        go.transform.position = point;
        go.transform.rotation = Quaternion.LookRotation(normal);

        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.4f;
        main.startSpeed = 3f;
        main.startSize = 0.05f;
        main.startColor = new Color(1f, 0.6f, 0.1f);
        main.maxParticles = 20;

        var emission = ps.emission;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 15) });
        emission.rateOverTime = 0;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 45f;

        Destroy(go, 1f);
    }
}