using UnityEngine;

[CreateAssetMenu(
    fileName = "AttackRangeUpgrade",
    menuName = "UmaSurvival/Upgrades/Weapon Attack Range"
)]

public class AttackRangeUpgrade : UpgradeDefinition
{
    [SerializeField] private WeaponId targetWeapon = WeaponId.BackKickWeapon;

    [SerializeField, Min(0.01f)]
    private float increaseRate = 0.2f;

    public override bool CanApply(UpgradeContext context)
    {
        if(context == null || context.Weapons == null)
        {
            return false;
        }

        if(increaseRate <= 0f)
        {
            return false;
        }

        if(!context.Weapons.TryGetWeapon(
                targetWeapon,
                out WeaponBase weapon
        ))
        {
            return false;
        }

        return weapon != null && weapon.AttackRange >0f;
    }

    public override bool TryApply(UpgradeContext context)
    {
        if(context == null || context.Weapons == null)
        {
            return false;
        }

        if(!context.Weapons.TryGetWeapon(
                targetWeapon, 
                out WeaponBase weapon))
        {
            return false;
        }

        if(weapon == null)
        {
            return false;
        }

        return weapon.TryIncreaseAttackRange(increaseRate);
    }
}
