using UnityEngine;

[CreateAssetMenu(fileName = "WeaponVisualData", menuName = "Weapons/Visual Data")]
public class WeaponVisualData : ScriptableObject {
    [Header("Impact Particles")]
    public Color impactColorStart   = new Color(1f, 0.6f, 0.1f);
    public Color impactColorEnd     = new Color(1f, 0.2f, 0f);
    public float impactSize         = 0.05f;
    public float impactLifetime     = 0.4f;
    public float impactSpeedMin     = 2f;   // Velocidad mínima de dispersión
    public float impactSpeedMax     = 6f;   // Velocidad máxima de dispersión
    public bool  impactFadeOut      = true; // Desvanecimiento suave al morir
    public bool  impactGrowsOverTime = false;
    public float impactFinalSize    = 0.2f; // Tamaño final si grows está activo
    public int impactParticleCount = 15;

    
    [Header("Impact Shape")]
    public bool use3DMesh         = false;
    public Mesh impactMesh;               // Solo se usa si use3DMesh = true
    public Sprite impactSprite;           // Solo se usa si use3DMesh = false


    [Header("Bullet")]
    public bool hasBulletDrop = false;    // La bala cae por gravedad
    
    [Header("Recoil Animation")]
    public float recoilDistance = 0.05f;  // Cuanto retrocede
    public float recoilRotation = 5f;     // Cuanto rota hacia arriba
    public float recoilSpeed    = 10f;    // Velocidad de retroceso
    public float returnSpeed    = 6f;     // Velocidad de vuelta
        

    [Header("Trail")]
    public Color trailColor       = new Color(1f, 0.5f, 0f);
    public float trailTime        = 0.15f;

    [Header("Materials")]
    public Material bulletMaterial; // Arrastra aquí el material de la bala
    public Material trailMaterial;  // Arrastra aquí el material del trail
    public Material impactMeshMaterial;  // Material de las partículas 3D
}