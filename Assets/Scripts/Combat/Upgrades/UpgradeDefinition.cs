using UnityEngine;

public abstract class UpgradeDefinition : ScriptableObject
{
    [SerializeField] private string displayName;

    [SerializeField, TextArea] 
    private string description;

    [SerializeField, Min(1)]
    private int maxRank = 5;

    public string DisplayName =>displayName;
    public string Description =>description;
    public int MaxRank => Mathf.Max(1, maxRank);


    //강화 후보에 넣기 전, 적용 가능 여부 판단
    public abstract bool CanApply(UpgradeContext context);


    // 적용 후 성공 여부 반환
    public abstract bool TryApply(UpgradeContext context);
}
