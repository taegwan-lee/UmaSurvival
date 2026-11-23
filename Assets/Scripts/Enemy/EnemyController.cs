using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 3f;
    
    
    private Rigidbody2D rb; //적 본인의 rigidbody

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation =true;
    }

    private void FixedUpdate()
    {   
        //죽거나 플레이어 없을 때 이동 정지
        if(player == null)
        {
            SetVelocity(Vector2.zero);
            return;
        }
        
        // 적 -> 플레이어 방향
        Vector2 direction = (Vector2)player.position - rb.position;
        
        SetVelocity(direction.normalized * moveSpeed);
    }

    private void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity =velocity;
    }

    private void OnDisable()
    {
        if(rb!=null)
        {
            SetVelocity(Vector2.zero);
        }
    }

    //EnemySpawner에서 spawn시 플레이어 추적용
    public void Initialize(Transform target)
    {
        player = target;
    }
    
}
