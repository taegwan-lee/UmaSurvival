using UnityEngine;

[CreateAssetMenu(fileName = "HorseShoeBurnUpgrade", menuName = "UmaSurvival/Upgrades/HorseShoe BurnUpgrade")]
public class HorseShoeBurnUpgrade : UpgradeDefinition
{
    public override bool CanApply(UpgradeContext context)
    {
        if(context == null || context.Weapons == null)
        {
            return false;
        }

        if(!context.Weapons.TryGetWeapon(
            WeaponId.HorseShoe, 
            out WeaponBase weapon))
        {
            return false;
        }

        HorseShoeWeapon horseShoe = weapon as HorseShoeWeapon;

        return horseShoe != null && !horseShoe.HasBurnHorseShoe;
    }

    public override bool TryApply(UpgradeContext context)
    {
        if(context == null || context.Weapons == null)
        {
            return false;
        }

        if(!context.Weapons.TryGetWeapon(
            WeaponId.HorseShoe,
            out WeaponBase weapon))
        {
                return false;
        }

        HorseShoeWeapon horseShoe = weapon as HorseShoeWeapon;

        return horseShoe != null && horseShoe.TryEnableBurnHorseshoe();
    }
}
