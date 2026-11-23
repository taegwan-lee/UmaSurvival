using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private string targetTag = "Player";

    private Health ownHealth;

    private void Awake()
    {
        ownHealth = GetComponent<Health>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDealDamage(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDealDamage(collision);
    }

    //부딪힌 오브젝트 데미지 줄지 말지 판단하기
    private void TryDealDamage(Collision2D collision)
    {
        if(!isActiveAndEnabled)
        {
            return;
        }
        if(ownHealth != null && ownHealth.IsDead)
        {
            return;
        }

        // 접촉한 오브젝트에서 Health 찾기
        Health targetHealth =
            collision.gameObject.GetComponentInParent<Health>();

        if(targetHealth ==null)
        {
            return;
        }

        if(!targetHealth.CompareTag(targetTag))
        {
            return;
        }

        targetHealth.TakeDamage(damage);
    }
}
