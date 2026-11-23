using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed =5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection;

    public Vector2 MoveInput => moveInput;
    public Vector2 LastMoveDirection => lastMoveDirection;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 중력과 회전 막기
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        lastMoveDirection = Vector2.right;
    }

    private void Update()
    {
        Vector2 input = Vector2.zero;
        Keyboard keyboard = Keyboard.current;

        if(keyboard != null){
            if(keyboard.wKey.isPressed) input.y +=1f;
            if(keyboard.sKey.isPressed) input.y -=1f;
            if(keyboard.aKey.isPressed) input.x -=1f;
            if(keyboard.dKey.isPressed) input.x +=1f;
        }


        moveInput = Vector2.ClampMagnitude(input,1f); //normalize
        if(moveInput != Vector2.zero)
        {  
            lastMoveDirection = moveInput;
        }
    }

    private void FixedUpdate()
    {
        SetVelocity(moveInput * moveSpeed);
    }

    private void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }
}
