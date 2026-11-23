using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private static readonly int IsMovingParameter = Animator.StringToHash("IsMoving");

    private PlayerController movement;
    private Animator animator;
    private SpriteRenderer sprite;
    private GameFlowController gameFlow;

    private void Awake()
    {
        movement = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        gameFlow = GetComponent<GameFlowController>();
    }

    private void LateUpdate()
    {
        if(gameFlow.State != GameState.Playing)
        {
            return;
        }

        Vector2 input = movement.MoveInput;
        animator.SetBool(IsMovingParameter, input != Vector2.zero);

        if(input.x != 0f)
        {
            sprite.flipX = input.x > 0f;
        }
    }
}
