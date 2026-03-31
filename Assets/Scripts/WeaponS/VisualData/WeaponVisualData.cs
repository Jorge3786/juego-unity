using UnityEngine;

[CreateAssetMenu(fileName = "WeaponVisualData", menuName = "Weapons/Visual Data")]
public class WeaponVisualData : ScriptableObject
    {
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
    

    [Header("Trail")]
    public Color trailColor       = new Color(1f, 0.5f, 0f);
    public float trailTime        = 0.15f;
}