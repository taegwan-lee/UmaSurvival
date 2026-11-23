using UnityEngine;

public class HairPinProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    private Rigidbody2D rb;
    private int damage;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 ownerDirection, int ownerDamage, float ownerSpeed)
    {
        rb.linearVelocity = ownerDirection*ownerSpeed;
        damage = ownerDamage;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health enemyHealth;
        enemyHealth = other.GetComponentInParent<Health>();
        if(enemyHealth == null)
        {
            return;
        }

        if(!enemyHealth.CompareTag("Enemy") || enemyHealth.IsDead)
        {
            return;
        }

        if(hasHit)
        {
            return;
        }
        
        hasHit = true;
        enemyHealth.TakeDamage(damage);

        Destroy(gameObject);
        return;
    }
}
