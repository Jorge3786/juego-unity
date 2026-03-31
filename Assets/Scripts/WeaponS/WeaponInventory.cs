using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInventory : MonoBehaviour
{
    public WeaponSlot[] slots;
    public int activeSlot = 0;

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) SetActiveSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SetActiveSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SetActiveSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SetActiveSlot(3);
    }

    public void SetActiveSlot(int index)
    {
        if (index >= slots.Length) return;
        activeSlot = index;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetActive(i == activeSlot);
        }
    }

    public bool AddWeapon(WeaponItem weapon)
    {
        foreach (var slot in slots)
        {
            if (slot.weapon == null)
            {
                slot.weapon = weapon;
                return true;
            }
        }
        return false;
    }
}