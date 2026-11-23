using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    private float damageBonus;

    public float DamageBonus=>damageBonus;
    public float DamageMultiplier => 1f + damageBonus;

    public void AddDamageBonus(float amount)
    {
        if(amount<0f)
        {
            return;
        }

        damageBonus += amount;
    }

    public int CalculateDamage(int baseDamage)
    {
        float finalDamae = baseDamage * DamageMultiplier;

        return Mathf.Max(0, Mathf.RoundToInt(finalDamae));
    }
}
