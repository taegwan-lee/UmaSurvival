using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField, Min(0.01f)] private float minAttackInterval = 0.1f;

    [SerializeField] private float attackRange = 6f;
    [SerializeField] private WeaponId weaponId = WeaponId.None;
    
    //애니메이션 용 지금 공격해도 되는 게임 상태인가?
    protected bool CanAttackNow => isActiveAndEnabled && 
    (ownerFlow == null || ownerFlow.IsPlaying) &&
    (ownerHealth == null || !ownerHealth.IsDead);

    //애니메이션 용 자식 무기가 다음 공격을 시작할 준비가 되었는가?
    protected virtual bool CanStartAttack => true;

    public float AttackInterval => attackInterval;
    public float MinAttackInterval => minAttackInterval;
    public bool CanReduceAttackInterval => attackInterval > minAttackInterval;
    public float AttackRange => attackRange;

    public WeaponId Id => weaponId;

    private float attackTimer;
    private Health ownerHealth;
    private PlayerCombatStats ownerStats;
    private GameFlowController ownerFlow;


    //무기에서 공격 배율 적용 되어 받도록
    protected int Damage
    {
        get
        {
            if(ownerStats==null)
            {
                return damage;
            }

            return ownerStats.CalculateDamage(damage);
        }
    }

    protected virtual void Awake()
    {
        ownerHealth = GetComponentInParent<Health>();
        ownerStats = GetComponentInParent<PlayerCombatStats>();
        ownerFlow = GetComponentInParent<GameFlowController>();

        //공격 간격 유효범위 보정
        minAttackInterval = Mathf.Max(0.01f, minAttackInterval);
        attackInterval = Mathf.Max(minAttackInterval, attackInterval);
    }

    private void Update()
    {   
       if(!CanAttackNow)
       {
            return;
       }

        attackTimer -= Time.deltaTime;
        
        if(attackTimer <= 0f && CanStartAttack)
        {
            Attack();
            attackTimer = attackInterval;
        }
    }   

    public bool TryReduceAttackInterval(float reductionRate)
    {
        if(reductionRate <= 0f || reductionRate >= 1f)
        {
            return false;
        }

        if(!CanReduceAttackInterval)//AttackInterval이 minInterval보다 작다면
        {
            return false;
        }

        float oldInterval = attackInterval;

        float newInterval  = Mathf.Max(minAttackInterval, oldInterval* (1f-reductionRate));

        if(newInterval >= oldInterval)
        {
            return false;
        }

        float remainingRatio = Mathf.Clamp01(attackTimer / oldInterval);

        attackInterval = newInterval;

        // 남은 대기 시간 조정
        attackTimer = newInterval * remainingRatio;

        return true;
    }

    public bool TryIncreaseAttackRange(float increaseRate)
    {
        if(increaseRate <= 0f || attackRange <=0f)
        {
            return false;
        }

        float newRange = attackRange * (1f + increaseRate);

        if(newRange <= attackRange)
        {
            return false;
        }

        attackRange = newRange;

        return true;
    }

    protected abstract void Attack();
    
}
