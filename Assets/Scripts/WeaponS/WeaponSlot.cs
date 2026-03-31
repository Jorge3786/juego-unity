using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    public WeaponItem weapon;
    public GameObject weaponModel; // Arrastra aquí el GameObject del arma

    public void SetActive(bool active)
    {
        if (weaponModel != null)
            weaponModel.SetActive(active);
        else
            gameObject.SetActive(active); // fallback si no hay modelo asignado
    }
}