using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private Transform weaponRoot;
    [SerializeField] private int maxWeaponCount = 6;

    private readonly Dictionary<WeaponId, WeaponBase> weapons = 
        new Dictionary<WeaponId, WeaponBase>();

    public int WeaponCount => weapons.Count;
    public int MaxWeaponCount => maxWeaponCount;

    public void Awake()
    {
        maxWeaponCount = Mathf.Max(1, maxWeaponCount);

        if(weaponRoot == null)
        {
            return;
        }

        WeaponBase[] startingWeapons = 
            weaponRoot.GetComponentsInChildren<WeaponBase>(true);

        foreach(WeaponBase weapon in startingWeapons)
        {
            if(!TryRegisterWeapon(weapon))
            {
                Debug.LogError(
                    "시작무기등록 실패"
                );
            }
        }
    }

    public bool HasWeapon(WeaponId weaponId)
    {
        return weapons.ContainsKey(weaponId);
    }

    public bool CanAddWeapon(WeaponId weaponId)
    {
        return weaponRoot != null  
            && weaponId != WeaponId.None
            && WeaponCount < maxWeaponCount
            && !HasWeapon(weaponId);
    }

    public bool TryGetWeapon(WeaponId weaponId, out WeaponBase weapon)
    {
            return weapons.TryGetValue(weaponId, out weapon);
    }

    public bool TryAddWeapon(WeaponBase weaponPrefab)
    {
        if(weaponPrefab == null ||
            !CanAddWeapon(weaponPrefab.Id))
            {
                return false;
            }
        
        WeaponBase newWeapon = Instantiate(
            weaponPrefab,
            weaponRoot
        );

        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        if(!TryRegisterWeapon(newWeapon))
        {
            Destroy(newWeapon.gameObject);
            return false;
        }

        return true;
    }

    private bool TryRegisterWeapon(WeaponBase weapon)
    {
        if(weapon == null || !CanAddWeapon(weapon.Id))
        {
            return false;
        }

        weapons.Add(weapon.Id, weapon);
        return true;
    }
}
