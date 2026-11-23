public class UpgradeContext
{
    public PlayerCombatStats CombatStats { get; }
    public WeaponInventory Weapons { get; }

    public UpgradeContext(PlayerCombatStats combatStats, WeaponInventory weapons)
    {
        CombatStats = combatStats;
        Weapons = weapons;
    }
}