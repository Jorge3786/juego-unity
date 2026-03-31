using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Weapon")]
public class WeaponItem : ScriptableObject
{
    public string weaponName;
    public Sprite icon; // icono para la UI
}