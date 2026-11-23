using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HorseShoeStrike : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [SerializeField] private SpriteRenderer rangeVisual;
    [SerializeField] private ParticleSystem impactParticles;
    [SerializeField] private LingeringDamageArea lingeringAreaPrefab;

    [SerializeField, Min(0f)]
    private float fallHeight = 3f;

    [SerializeField, Min(0.01f), Tooltip("Elapsed time from launch to impact.")]
    private float fallDuration = 0.35f;

    [SerializeField, Tooltip("Height as a fraction of Fall Height, from launch (0) to impact (1).")]
    private AnimationCurve heightOverFlight = new AnimationCurve(
        new Keyframe(0f, 0.1f, 0f, 4.5f),
        new Keyframe(0.29f, 1f, 0f, 0f),
        new Keyframe(0.49f, 0.98f, 0f, 0f),
        new Keyframe(1f, 0f, -3.9f, 0f)
    );

    [SerializeField, Min(0f), FormerlySerializedAs("impactDisplayDuration")]
    private float impactFadeDuration = 0.4f;

    private SpriteRenderer visualRenderer;
    private Color visualColor;
    private Color rangeColor;
    private int damage;
    private float impactRadius;
    private LayerMask enemyLayer;
    private GameFlowController gameFlow;

    private bool hasBurn;
    private float flightElapsed;
    private float impactElapsed;
    private bool initialized;
    private bool hasImpacted;

    private readonly HashSet<Health> hitTargets =
        new HashSet<Health>();

    private void Awake()
    {
        if (visual != null)
        {
            visualRenderer = visual.GetComponentInChildren<SpriteRenderer>(true);
            if (visualRenderer != null)
            {
                visualColor = visualRenderer.color;
            }
        }

        if (rangeVisual != null)
        {
            rangeColor = rangeVisual.color;
        }
    }

    public void Initialize(
        int damageAmount,
        float radius,
        LayerMask targetLayers,
        GameFlowController ownerFlow,
        bool enableHasBurn = false)
    {
        if (visual == null || visual == transform)
        {
            Debug.LogError("HorseshoeStrike requires a separate scene object for its visual.", this);

            Destroy(gameObject);
            return;
        }

        damage = Mathf.Max(0, damageAmount);
        impactRadius = Mathf.Max(0.01f, radius);
        enemyLayer = targetLayers;
        gameFlow = ownerFlow;
        hasBurn = enableHasBurn;

        flightElapsed = 0f;
        impactElapsed = 0f;
        hasImpacted = false;
        hitTargets.Clear();

        visual.gameObject.SetActive(true);
        if (visualRenderer != null)
        {
            visualRenderer.color = visualColor;
        }
        if (rangeVisual != null)
        {
            rangeVisual.enabled = true;
        }
        if (impactParticles != null)
        {
            impactParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        ResizeRangeVisual();
        ApplyFlightPose(0f);

        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (gameFlow != null && !gameFlow.CanSimulateWorld)
        {
            return;
        }

        if (hasImpacted)
        {
            UpdateImpactVisuals();
            return;
        }

        flightElapsed += Time.deltaTime;

        float progress = Mathf.Clamp01(
            flightElapsed / Mathf.Max(0.01f, fallDuration)
        );

        if (progress >= 1f)
        {
            visual.localPosition = Vector3.zero;
            Impact();
            return;
        }

        ApplyFlightPose(progress);
    }

    private void ApplyFlightPose(float progress)
    {
        // 착지점은 고정하고, 편자 그림만 높이 곡선을 따라 이동합니다.
        float height = Mathf.Max(0f, heightOverFlight.Evaluate(progress)) * fallHeight;
        visual.localPosition = Vector3.up * height;

        if (rangeVisual != null)
        {
            float warning = Mathf.InverseLerp(0.35f, 1f, progress);
            Color color = rangeColor;
            color.a = Mathf.Clamp01(rangeColor.a * Mathf.Lerp(0.75f, 1.85f, warning * warning));
            rangeVisual.color = color;
        }
    }

    private void Impact()
    {
        // 다음 프레임에 다시 타격하지 않도록 먼저 표시
        hasImpacted = true;

        if (rangeVisual != null)
        {
            rangeVisual.enabled = false;
        }
        if (impactParticles != null)
        {
            impactParticles.Play(true);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            (Vector2)transform.position,
            impactRadius,
            enemyLayer
        );

        foreach (Collider2D candidate in colliders)
        {
            if (candidate == null)
            {
                continue;
            }

            Health health = candidate.GetComponentInParent<Health>();

            if (health == null ||
                !health.isActiveAndEnabled ||
                health.IsDead ||
                !health.CompareTag("Enemy"))
            {
                continue;
            }

            // 같은 적의 Collider가 여러 개여도 한 번만 피해주기
            if (hitTargets.Add(health))
            {
                health.TakeDamage(damage);
            }
        }

        //잔상 공격 업그레이드
        if (hasBurn && lingeringAreaPrefab != null)
        {
            LingeringDamageArea area = Instantiate(
                lingeringAreaPrefab,
                transform.position,
                Quaternion.identity
            );

            int lingeringDamage = Mathf.RoundToInt(damage * 0.25f);

            area.Initialize(
                lingeringDamage,
                impactRadius,
                enemyLayer,
                gameFlow
            );
        }
    }

    private void UpdateImpactVisuals()
    {
        impactElapsed += Time.deltaTime;

        if (visual.gameObject.activeSelf)
        {
            float progress = impactFadeDuration > 0f
                ? Mathf.Clamp01(impactElapsed / impactFadeDuration)
                : 1f;

            if (visualRenderer != null)
            {
                Color fadedColor = visualColor;
                fadedColor.a *= 1f - Mathf.SmoothStep(0f, 1f, progress);
                visualRenderer.color = fadedColor;
            }

            if (progress < 1f)
            {
                return;
            }

            visual.gameObject.SetActive(false);
        }

        if (impactParticles == null || !impactParticles.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }

    private void ResizeRangeVisual()
    {
        if (rangeVisual == null || rangeVisual.sprite == null)
        {
            return;
        }

        Vector3 spriteSize = rangeVisual.sprite.bounds.size;
        float diameter = impactRadius * 2f;

        // 원 그림의 크기를 실제 피해 반경에 맞춥니다.
        rangeVisual.transform.localScale = new Vector3(
            diameter / Mathf.Max(0.0001f, spriteSize.x),
            diameter / Mathf.Max(0.0001f, spriteSize.y),
            1f
        );
    }
}
