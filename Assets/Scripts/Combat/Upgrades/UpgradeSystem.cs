using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UpgradeSystem : MonoBehaviour
{
    [SerializeField] 
    private List<UpgradeDefinition> upgradePool = 
        new List<UpgradeDefinition>();

    [SerializeField, Min(1)]
    private int choiceCount = 3;

    // 현재까지 각 강화를 획득한 횟수
    private readonly Dictionary<UpgradeDefinition, int> ranks = 
        new Dictionary<UpgradeDefinition, int>();
    
    // 현재 선택할 수 있는 강화
    private readonly List<UpgradeDefinition> currentChoices = 
        new List<UpgradeDefinition>();

    private UpgradeContext context;
    private bool isApplying;

    public bool IsChoosing { get; private set; }
    public int CurrentOfferId { get; private set; }

    public IReadOnlyList<UpgradeDefinition> CurrentChoices => currentChoices;

    private void Awake()
    {
        choiceCount = Mathf.Max(1, choiceCount);

        PlayerCombatStats stats = GetComponent<PlayerCombatStats>();

        WeaponInventory inventory = GetComponent<WeaponInventory>();

        if(stats == null || inventory== null)
        {
            Debug.LogError(
                "같은 오브젝트에 PlayerCombatStats와 WeaponInventory 필요",
                this
            );
            return;
        }

        context = new UpgradeContext(stats, inventory);
    }

    public int GetRank(UpgradeDefinition upgrade)
    {
        if(upgrade == null)
        {
            return 0;
        }

        return ranks.TryGetValue(upgrade, out int rank)
            ? rank
            : 0;
    }

    public bool TryCreateChoices()
    {
        if(context ==null || IsChoosing || isApplying)
        {
            return false;
        }

        currentChoices.Clear();

        List<UpgradeDefinition> candidates = 
            new List<UpgradeDefinition>();

        foreach(UpgradeDefinition upgrade in upgradePool)
        {
            if(CanOffer(upgrade) && !candidates.Contains(upgrade))
            {
                candidates.Add(upgrade);
            }
        }

        if(candidates.Count == 0)
        {
            return false;
        }

        int count = Mathf.Min(choiceCount, candidates.Count);

        for(int i =0; i< count; i++)
        {
            int randomIndex = Random.Range(0, candidates.Count);

            currentChoices.Add(candidates[randomIndex]);

            candidates.RemoveAt(randomIndex);
        }

        CurrentOfferId++;
        IsChoosing = true;
        return true;
    }

    // 선택 중단 시 현재 강화 후보 정리
    public void CancelChoices()
    {
        if(isApplying)
        {
            return;
        }

        IsChoosing = false;
        currentChoices.Clear();
    }

    public bool TryChoose(int offerId, int index)
    {
        if(!IsChoosing ||
            isApplying ||
            offerId != CurrentOfferId ||
            index <0 ||
            index >=currentChoices.Count)
        {
            return false;
        }

        UpgradeDefinition upgrade = currentChoices[index];

        if(!CanOffer(upgrade))
        {
            return false;
        }

        isApplying = true;

        try 
        {
            if(!upgrade.TryApply(context))
            {
                return false;
            }

            ranks[upgrade] = GetRank(upgrade) +1;

            IsChoosing = false;
            currentChoices.Clear();

            return true;
        }
        finally
        {
            isApplying = false;
        }
    }

    private bool CanOffer(UpgradeDefinition upgrade)
    {
        return context != null
            && upgrade != null
            && GetRank(upgrade) < upgrade.MaxRank
            && upgrade.CanApply(context);
    }
}
