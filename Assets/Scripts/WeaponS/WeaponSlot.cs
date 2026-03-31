using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    public WeaponItem weapon;  // volver a añadir
    // public GameObject weaponModel; // opcional

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}