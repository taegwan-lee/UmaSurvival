using System;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{   
    [SerializeField] private int requiredExperience = 5;
    [SerializeField] private int PerLevelUp = 3;
   
    private int level =1;
    private int currentExperience;

    public int Level => level;
    public int CurrentExperience => currentExperience;
    public int RequiredExperience => requiredExperience;
    public bool CanLevelUp { get; private set; } = true;

    //레벨업 이벤트
    public event Action<int> LeveledUp;

    public void SetLevelUpAllowed(bool allowed)
    {
        CanLevelUp = allowed;
    }

    public void AddExperience(int amount)
    {
        if(amount <=0)
        {
            return;
        }


        currentExperience += amount;

        if(CanLevelUp && currentExperience >= requiredExperience)
        {
            currentExperience -= requiredExperience;
            LevelUp();
        }

        //확인용
        Debug.Log($"경험치 +{amount}, 현재 경험치 : {currentExperience}");
    }

    private void LevelUp()
    {
        level++;

        requiredExperience += PerLevelUp;

        Debug.Log($"레벨업! 현재 레벨 : {level}");

        LeveledUp?.Invoke(level);
    }


}
