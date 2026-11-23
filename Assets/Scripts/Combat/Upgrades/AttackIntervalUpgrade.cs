using UnityEngine;

[CreateAssetMenu(
    fileName = "AttackIntervalUpgrade",
    menuName = "UmaSurvival/Upgrades/Weapon Attack Interval")]

public class AttackIntervalUpgrade : UpgradeDefinition
{
    [SerializeField] private WeaponId targetWeapon = WeaponId.Projectile;

    [SerializeField] private float reductionRate = 0.1f;

    public override bool CanApply(UpgradeContext context)
    {
        if(context == null || context.Weapons == null)
        {
            return false;
        }

        if(reductionRate <= 0f || reductionRate >= 1f)
        {
            return false;
        }

        if(!context.Weapons.TryGetWeapon(
            targetWeapon, 
            out WeaponBase weapon))
        {
            return false;
        }

        return weapon != null && weapon.CanReduceAttackInterval;
    }

    public override bool TryApply(UpgradeContext context)
    {
          if (context == null || context.Weapons == null)
        {
            return false;
        }

        if (!context.Weapons.TryGetWeapon(
                targetWeapon,
                out WeaponBase weapon))
        {
            return false;
        }

        if (weapon == null)
        {
            return false;
        }
        
        return weapon.TryReduceAttackInterval(reductionRate);

    }
}
