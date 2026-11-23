using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    private static readonly int DeathStateHash = 
        Animator.StringToHash("Base Layer.Death");

    private Health health;
    private PlayerController movement;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        health = GetComponent<Health> ();
        movement = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        health.Died += HandleDeath;
    }

    private void OnDisable()
    {
        health.Died -= HandleDeath;
    }

    private void HandleDeath()
    {
        movement.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.Play(DeathStateHash, 0, 0f);
  
    }
}
