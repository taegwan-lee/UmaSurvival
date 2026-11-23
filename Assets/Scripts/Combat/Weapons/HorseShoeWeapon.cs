using UnityEngine;

public class HorseShoeWeapon : WeaponBase
{
    [SerializeField] private HorseShoeStrike strikePrefab;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField, Min(0.01f)] private float impactRadius = 1.2f;

    private bool hasBurnHorseShoe;
    private GameFlowController gameFlow;

    public bool HasBurnHorseShoe => hasBurnHorseShoe;

    protected override void Awake()
    {
        base.Awake();

        hasBurnHorseShoe = false;
        gameFlow = GetComponentInParent<GameFlowController>();
    }

    protected override void Attack()
    {
        if(strikePrefab == null)
        {
            return;
        }

        // 플레이어 주변 원 안에서 무작위 위치 선정(공격)
        Vector2 offset = Random.insideUnitCircle * Mathf.Max(0f, AttackRange);

        Vector3 landingPosition = transform.position + new Vector3(offset.x, offset.y, 0f);

        HorseShoeStrike strike = Instantiate(
            strikePrefab,
            landingPosition,
            Quaternion.identity
        );

        strike.Initialize(
            Damage,
            impactRadius,
            enemyLayer,
            gameFlow,
            hasBurnHorseShoe
        );
    }

    // 업그레이드 가능 조건용
    public bool TryEnableBurnHorseshoe()
    {
        if(hasBurnHorseShoe)
        {
            return false;
        }

        hasBurnHorseShoe = true;
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            Mathf.Max(0f, AttackRange)
        );
    }
}
