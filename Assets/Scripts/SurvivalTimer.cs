using UnityEngine;

public class SurvivalTimer : MonoBehaviour
{
    private GameFlowController gameFlow;
    public float ElapsedSeconds { get; private set;}

    private void Awake()
    {
        gameFlow = GetComponent<GameFlowController>();

        ElapsedSeconds = 0f;
    }

    private void Update()
    {   
        if(!gameFlow.IsPlaying)
        {
            return;
        }

        ElapsedSeconds += Time.deltaTime;
    }
}
