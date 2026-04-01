using UnityEngine;
using System.Collections;

public class ProjectileBullet : MonoBehaviour
{
    [SerializeField] private WeaponVisualData visualData;
    [SerializeField] private float ignoreCollisionTime = 0.1f;
    private TrailRenderer trail;
    private bool _canCollide = false;

    void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();

        if (trail == null)
            Debug.LogWarning("[ProjectileBullet] No se encontró TrailRenderer en los hijos.", this);
        else
            Debug.Log("[ProjectileBullet] TrailRenderer encontrado: " + trail.gameObject.name);

        if (trail != null && visualData != null)
        {
            if (visualData.trailMaterial != null)
                trail.material = visualData.trailMaterial;

            trail.startColor = visualData.trailColor;
            trail.endColor   = new Color(visualData.trailColor.r,
                                         visualData.trailColor.g,
                                         visualData.trailColor.b, 1f);
            trail.time       = visualData.trailTime;
        }

        StartCoroutine(EnableCollision());
    }

    IEnumerator EnableCollision()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        yield return new WaitForSeconds(ignoreCollisionTime);
        if (col != null) col.enabled = true;
        _canCollide = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!_canCollide) return;
        SpawnImpact(collision.contacts[0].point, collision.contacts[0].normal);
        Destroy(gameObject);
    }

    void SpawnImpact(Vector3 point, Vector3 normal)
    {
        if (trail != null)
        {
            trail.transform.SetParent(null);
            trail.autodestruct = true;
            Destroy(trail.gameObject, trail.time + 0.5f);
        }

        var go = new GameObject("BulletImpact");
        float radius = (visualData != null) ? visualData.generateParticlesRadius : 0f;

        Vector3 spawnPosition;

        if (radius <= 0f)
        {
            // Impacto EXACTO (pistola)
            spawnPosition = point + normal * 0.05f;
        }
        else
        {
            Vector3 randomOffset = Random.insideUnitSphere * radius;

            // Evita que las partículas se metan dentro de la superficie
            if (Vector3.Dot(randomOffset, normal) < 0)
                randomOffset = -randomOffset;

            spawnPosition = point + normal * 0.05f + randomOffset;
        }

        go.transform.position = spawnPosition;
        go.transform.rotation = Quaternion.LookRotation(normal);

        var ps  = go.AddComponent<ParticleSystem>();
        var psr = go.GetComponent<ParticleSystemRenderer>();

        if (visualData != null && visualData.use3DMesh)
        {
            psr.renderMode     = ParticleSystemRenderMode.Mesh;
            psr.mesh           = visualData.impactMesh != null
                ? visualData.impactMesh
                : Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            psr.material = visualData.impactMeshMaterial != null
                ? visualData.impactMeshMaterial
                : new Material(Shader.Find("Standard"));
        }
        else
        {
            psr.renderMode = ParticleSystemRenderMode.Billboard;
            psr.material   = new Material(Shader.Find("Particles/Standard Unlit"));
            psr.material.SetFloat("_Mode", 2f);
            psr.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            psr.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            psr.material.SetInt("_ZWrite", 0);
            psr.material.SetInt("_ZTest",  (int)UnityEngine.Rendering.CompareFunction.Always);
            psr.material.EnableKeyword("_ALPHABLEND_ON");

            if (visualData != null && visualData.impactSprite != null)
                psr.material.SetTexture("_MainTex", visualData.impactSprite.texture);
        }

        var main           = ps.main;
        main.loop          = false;
        main.startLifetime = visualData != null ? visualData.impactLifetime : 0.4f;
        main.startSpeed    = visualData != null
            ? new ParticleSystem.MinMaxCurve(visualData.impactSpeedMin, visualData.impactSpeedMax)
            : new ParticleSystem.MinMaxCurve(2f, 6f);
        main.startSize     = visualData != null ? visualData.impactSize : 0.05f;
        main.startColor    = visualData != null
            ? new ParticleSystem.MinMaxGradient(
                new Color(visualData.impactColorStart.r, visualData.impactColorStart.g, visualData.impactColorStart.b, 1f),
                new Color(visualData.impactColorEnd.r,   visualData.impactColorEnd.g,   visualData.impactColorEnd.b,   1f))
            : new ParticleSystem.MinMaxGradient(Color.yellow, Color.red);
        main.maxParticles  = visualData != null ? visualData.impactParticleCount : 15;

        if (visualData != null && visualData.use3DMesh)
        {
            var rot          = ps.rotationOverLifetime;
            rot.enabled      = true;
            rot.separateAxes = true;
            rot.x            = new ParticleSystem.MinMaxCurve(-180f, 180f);
            rot.y            = new ParticleSystem.MinMaxCurve(-180f, 180f);
            rot.z            = new ParticleSystem.MinMaxCurve(-180f, 180f);
        }

        if (visualData != null && visualData.impactFadeOut)
        {
            var colorOverLifetime     = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;

            Gradient gradient = new Gradient();
            gradient.mode = GradientMode.Blend;
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(visualData.impactColorStart, 0f),
                    new GradientColorKey(visualData.impactColorEnd,   1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 0.6f),
                    new GradientAlphaKey(0f, 1f)
                }
            );

            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        }

        if (visualData != null && visualData.impactGrowsOverTime)
        {
            var sizeOverLifetime     = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, visualData.impactSize);
            curve.AddKey(1f, visualData.impactFinalSize);

            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);
        }

        var emission = ps.emission;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)(visualData != null ? visualData.impactParticleCount : 15)) });
        emission.rateOverTime = 0;

        var shape            = ps.shape;
        shape.shapeType      = ParticleSystemShapeType.Cone;
        shape.angle          = 45f;
        shape.alignToDirection = true;

        Destroy(go, visualData != null ? visualData.impactLifetime + 0.5f : 1f);
    }
}