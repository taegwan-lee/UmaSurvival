using UnityEngine;

//가까운 적 자동 추적 무기(직선 발사)
public class ProjectileWeapon : WeaponBase
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private LayerMask enemyLayer;

    protected override void Attack()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        // 공격할 때마다 가장 가까운 적을 찾습니다.
        Transform target = FindNearestTarget();

        if (target == null)
        {
            return;
        }

        Vector2 direction =
            (Vector2)target.position - (Vector2)transform.position;

        Projectile projectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        projectile.Initialize(direction, Damage);
    }

    private Transform FindNearestTarget()
    {
        // 사거리 안에 있는 Enemy 레이어의 Collider2D를 가져옵니다.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            AttackRange,
            enemyLayer
        );

        Transform nearestTarget = null;
        float nearestSqrDistance = AttackRange * AttackRange;

        foreach (Collider2D candidate in colliders)
        {
            Health health = candidate.GetComponentInParent<Health>();

            // 살아 있는 적만 대상으로 삼습니다.
            if (health == null ||
                !health.isActiveAndEnabled ||
                health.IsDead ||
                !health.CompareTag("Enemy"))
            {
                continue;
            }

            Vector2 offset =
                (Vector2)health.transform.position -
                (Vector2)transform.position;

            float sqrDistance = offset.sqrMagnitude;

            // 지금까지 찾은 적보다 가까우면 대상을 교체합니다.
            if (sqrDistance <= nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearestTarget = health.transform;
            }
        }

        return nearestTarget;
    }
}
