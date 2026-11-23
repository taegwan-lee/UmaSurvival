using UnityEngine;

[CreateAssetMenu(
    fileName = "GlobalDamageUpgrade",
    menuName = "UmaSurvival/Upgrades/Global Damage")]
public class GlobalDamageUpgrade : UpgradeDefinition
{
    [SerializeField, Min(0f)]
    private float damageBonus = 0.1f;

    public override bool CanApply(UpgradeContext context)
    {
        return context != null
            && context.CombatStats != null  
            && damageBonus > 0f;
    }

    public override bool TryApply(UpgradeContext context)
    {
        if(!CanApply(context))
        {
            return false;
        }

        context.CombatStats.AddDamageBonus(damageBonus);
        return true;
    }
}
