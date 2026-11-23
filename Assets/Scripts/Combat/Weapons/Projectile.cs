using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;
    private int damage;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    // 무기가 방향, 피해량 전달
    public void Initialize(Vector2 direction, int damage)
    {
        this.damage = damage;

        rb.linearVelocity = direction.normalized * moveSpeed;

        // 빗나가도 일정 시간이 지나면 제거
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(hasHit)
        {
            return;
        }

        Health health = other.GetComponentInParent<Health>();

        if(health == null ||
        !health.CompareTag("Enemy")||
        health.IsDead)
        {
            return;
        }

        hasHit = true;

        health.TakeDamage(damage);
        Destroy(gameObject);
    }
}
