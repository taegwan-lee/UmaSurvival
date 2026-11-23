using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(
    typeof(PlayerExperience),
    typeof(UpgradeSystem),
    typeof(GameFlowController))]
public class LevelUpController : MonoBehaviour
{
    private PlayerExperience experience;
    private UpgradeSystem upgradeSystem;
    private GameFlowController gameFlow;
    private Health health;

    public bool IsChoosing { get; private set; }
    public int CurrentOfferId { get; private set; }

    public IReadOnlyList<UpgradeDefinition> CurrentChoices =>
        upgradeSystem.CurrentChoices;
    
    public event Action SelectionOpened;
    public event Action SelectionClosed;

    private void Awake()
    {
        experience = GetComponent<PlayerExperience>();
        upgradeSystem = GetComponent<UpgradeSystem>();
        gameFlow = GetComponent<GameFlowController>();
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        experience.LeveledUp += HandleLevelUp;
        health.Died += HandleDeath;

        if(health.IsDead)
        {
            HandleDeath();
        }
    }

    private void OnDisable()
    {
        if(experience != null)
        {
            experience.LeveledUp -= HandleLevelUp;
        }

        if(health != null)
        {
            health.Died -= HandleDeath;
        }

        FinishSelection();
    }

    private void Update()
    {
        if(!IsChoosing)
        {
            return;
        }
    }

    private void HandleLevelUp(int newLevel)
    {
        if(IsChoosing || !gameFlow.IsPlaying || health.IsDead)
        {
            return;
        }

        experience.SetLevelUpAllowed(false);

        if(!gameFlow.TryBeginUpgradeSelection())
        {
            experience.SetLevelUpAllowed(gameFlow.IsPlaying && !health.IsDead);

            return;
        }

        IsChoosing = true;

        //더 이상 선택할 강화가 없으면 진행 재개
        if(!OpenChoices())
        {
            FinishSelection();
        }
    }

    public bool TrySelect(int offerId, int index)
    {
        if(!IsChoosing || 
        health.IsDead || 
        gameFlow.State != GameState.UpgradeSelection ||
        offerId != CurrentOfferId ||
        index <0 ||
        index >= CurrentChoices.Count)
        {
            return false;
        }

        if(upgradeSystem.TryChoose(offerId, index))
        {
            FinishSelection();
            return true;
        }

        upgradeSystem.CancelChoices();

        if(!IsChoosing ||
        health.IsDead ||
        gameFlow.State != GameState.UpgradeSelection ||
        !OpenChoices())
        {
            FinishSelection();
        }

        return false;
    }

    private bool OpenChoices()
    {
        if(!upgradeSystem.TryCreateChoices())
        {
            return false;
        }

        CurrentOfferId = upgradeSystem.CurrentOfferId;

        SelectionOpened?.Invoke();
        return true;
    }

    private void FinishSelection()
    {
        if(!IsChoosing)
        {
            return;
        }

        IsChoosing = false;

        upgradeSystem.CancelChoices();
        gameFlow.EndUpgradeSelection();

        experience.SetLevelUpAllowed(
            gameFlow.IsPlaying && !health.IsDead
        );

        SelectionClosed?.Invoke();
    }

    private void HandleDeath()
    {
        experience.SetLevelUpAllowed(false);
        FinishSelection();
    }
}
