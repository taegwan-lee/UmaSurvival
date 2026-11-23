using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BackKickWeapon : WeaponBase
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField, Min(0.1f)] private float heightRatio = 0.4f;

    [SerializeField] private SpriteRenderer kickRenderer;
    [SerializeField] private AnimationClip kickClip;

    // 이미지 주변의 투명 여백을 보정하는 표시 배율입니다.
    [SerializeField] private Vector2 visualScale = new Vector2(2.2f, 2f);

    private static readonly int KickStateHash = Animator.StringToHash("Base Layer.Kick");

    private Animator animator;

    // 좌,우 방향 공격용
    private int nextDirection = -1;
    private int attackDirection;
    private Vector2 attackSize;
    private int attackDamage;

    private bool isAttacking;
    private bool hasHit;

    private readonly HashSet<Health> hitEnemies = new HashSet<Health>();

    protected override bool CanStartAttack => !isAttacking;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();

        if(kickRenderer == null ||
           kickClip == null ||
           animator.runtimeAnimatorController == null)
        {
            Debug.LogError("발차기 Render, Clip, Animator Cotroller 미연결", this);

            enabled = false;
            return;
        }

        kickRenderer.enabled = false;
    }

    protected override void Attack()
    {
        if (AttackRange <= 0f ||
            kickClip.length <= 0f ||
            kickRenderer.sprite == null)
        {
            return;
        }

        attackDirection = nextDirection;
        nextDirection *= -1;

        attackSize = GetAttackSize();
        attackDamage = Damage;

        isAttacking = true;
        hasHit = false;

        // 그림을 이번 공격 방향에 배치합니다.
        Transform visual = kickRenderer.transform;

        // 타격 판정에 사용하는 것과 같은 중심 좌표입니다.
        Vector2 center = GetAttackCenter(attackDirection, attackSize.x);

        visual.position = new Vector3(
            center.x,
            center.y,
            transform.position.z
        );

        kickRenderer.flipX = attackDirection < 0;

        Vector3 spriteSize = kickRenderer.sprite.bounds.size;
        Vector3 parentScale = visual.parent.lossyScale;

        // 부모에게서 물려받는 Scale을 나누어 보정합니다.
        visual.localScale = new Vector3(
            attackSize.x * visualScale.x / spriteSize.x / parentScale.x,
            attackSize.y * visualScale.y / spriteSize.y / parentScale.y,
            1f
        );

        // 공격 간격이 짧아지면 애니메이션도 빠르게 재생합니다.
        float availableTime =
            Mathf.Max(0.01f, AttackInterval * 0.8f);

        animator.speed =
            Mathf.Max(1f, kickClip.length / availableTime);

        kickRenderer.enabled = true;
        animator.Play(KickStateHash, 0, 0f);
    }

    // Animation Event에서 호출합니다.
    public void ApplyHit()
    {
        if (!isAttacking || hasHit || !CanAttackNow)
        {
            return;
        }

        hasHit = true;
        hitEnemies.Clear();

        Vector2 center =
            GetAttackCenter(attackDirection, attackSize.x);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            center,
            attackSize,
            0f,
            enemyLayer
        );

        foreach (Collider2D candidate in colliders)
        {
            Health health =
                candidate.GetComponentInParent<Health>();

            if (health == null ||
                !health.isActiveAndEnabled ||
                health.IsDead ||
                !health.CompareTag("Enemy"))
            {
                continue;
            }

            if (!hitEnemies.Add(health))
            {
                continue;
            }

            health.TakeDamage(attackDamage);
        }
    }

    // Animation Event에서 호출합니다.
    public void FinishAttack()
    {
        isAttacking = false;

        if (kickRenderer != null)
        {
            kickRenderer.enabled = false;
        }
    }

    private void OnDisable()
    {
        FinishAttack();
        hasHit = false;
        hitEnemies.Clear();
    }

    private Vector2 GetAttackSize()
    {
        return new Vector2(
            AttackRange,
            AttackRange * heightRatio
        );
    }

    private Vector2 GetAttackCenter(int direction, float range)
    {
        Vector2 offset =
            Vector2.right * direction * (range * 0.5f);

        return (Vector2)transform.position + offset;
    }

    private void OnDrawGizmosSelected()
    {
        if (AttackRange <= 0f)
        {
            return;
        }

        int direction = isAttacking
            ? attackDirection
            : nextDirection;

        Vector2 size = isAttacking
            ? attackSize
            : GetAttackSize();

        Vector2 center = GetAttackCenter(direction, size.x);

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            new Vector3(center.x, center.y, transform.position.z),
            new Vector3(size.x, size.y, 0f)
        );
    }
}