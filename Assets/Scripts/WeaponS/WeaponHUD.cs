using UnityEngine;
using UnityEngine.UI;

public class WeaponHUD : MonoBehaviour
{
    public WeaponInventory inventory;
    public SlotUI[] slotUIs; // un SlotUI por cada slot

    void Update()
    {
        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (i >= inventory.slots.Length) break;

            WeaponSlot slot = inventory.slots[i];
            bool hasWeapon = slot.weapon != null;
            bool isActive = i == inventory.activeSlot;

            // Solo mostrar slots que tengan arma
            slotUIs[i].gameObject.SetActive(hasWeapon);

            if (hasWeapon)
            {
                slotUIs[i].icon.sprite = slot.weapon.icon;
                slotUIs[i].highlight.SetActive(isActive);
            }
        }
    }
}