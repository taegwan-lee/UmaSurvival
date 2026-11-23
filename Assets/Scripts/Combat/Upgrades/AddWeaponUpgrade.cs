using UnityEngine;
[CreateAssetMenu(
    fileName = "AddWeaponUpgrade",
    menuName = "UmaSurvival/Upgrades/Add Weapon"
)]

public class AddWeaponUpgrade : UpgradeDefinition
{
    [SerializeField] private WeaponBase weaponPrefab;

    public override bool CanApply(UpgradeContext context)
    {
        if(context == null || context.Weapons == null)
        {
            return false;
        }

        if(weaponPrefab == null)
        {
            return false;
        }

        return context.Weapons.CanAddWeapon(weaponPrefab.Id);
    }

    public override bool TryApply(UpgradeContext context)
    {
        if(!CanApply(context))
        {
            return false;
        }

        return context.Weapons.TryAddWeapon(weaponPrefab);
    }
}
