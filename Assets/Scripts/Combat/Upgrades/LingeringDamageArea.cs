using UnityEngine;
using System.Collections.Generic;

public class LingeringDamageArea : MonoBehaviour
{
    [SerializeField] private float burningDuration = 3f;

    [SerializeField] private float tickInterval = 0.5f;

    private int damage;
    private float radius;
    private LayerMask enemyLayer;
    private GameFlowController gameFlow;

    private float elapsedTime;
    private float nextTickTime;
    private bool initialized;

    private readonly HashSet<Health> hitTargets = new HashSet<Health>();

    public void Initialize(
        int damageAmount,
        float impactRadius,
        LayerMask targetLayers,
        GameFlowController ownerFlow)
    {
        damage = damageAmount;
        radius = impactRadius;
        enemyLayer = targetLayers;
        gameFlow = ownerFlow;

        burningDuration = Mathf.Max(0.01f, burningDuration);
        tickInterval = Mathf.Max(0.01f, tickInterval);

        elapsedTime = 0f;
        nextTickTime = tickInterval;

        ResizeVisual();

        initialized = true;
    }

    private void Update()
    {
        if(!initialized)
        {
            return;
        }

        if( gameFlow != null && !gameFlow.CanSimulateWorld)
        {
            return;
        }

        elapsedTime = Mathf.Min(
            elapsedTime + Time.deltaTime,
            burningDuration
        );

        while(elapsedTime >= nextTickTime)
        {
            ApplyTickDamage();
            nextTickTime += tickInterval;
        }

        if(elapsedTime >= burningDuration)
        {
            Destroy(gameObject);
        }

    }

    private void ApplyTickDamage()
    {
            hitTargets.Clear();

            Collider2D[] colliders = Physics2D.OverlapCircleAll(
                (Vector2)transform.position,
                radius,
                enemyLayer
            );

            foreach(Collider2D candidate in colliders)
            {
                if(candidate == null)
                {
                    continue;
                }

                Health health = candidate.GetComponentInParent<Health>();

                if(health == null ||
                !health.isActiveAndEnabled ||
                health.IsDead ||
                !health.CompareTag("Enemy"))
                {
                    continue;
                }

                if(hitTargets.Add(health))
                {
                    health.TakeDamage(damage);
                }
            }
    }

    private void ResizeVisual()
    {
        SpriteRenderer areaVisual = GetComponent<SpriteRenderer>();

        Vector3 spriteSize = areaVisual.sprite.bounds.size;
        float diameter = radius * 2f;

        transform.localScale = new Vector3(
            diameter / Mathf.Max(0.0001f, spriteSize.x),
            diameter / Mathf.Max(0.0001f, spriteSize.y),
            1f
        );
    }
}
