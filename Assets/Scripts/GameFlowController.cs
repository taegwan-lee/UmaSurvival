using UnityEngine;

public enum GameState
{
    Playing,
    UpgradeSelection,
    Event,
    GameOver,
    Dying
}

public class GameFlowController : MonoBehaviour
{
    [SerializeField]
    private float deathTimeScale = 0.2f;
    
    [SerializeField]
    private float dyingDuration = 3f;

    private Health health;
    private float baseFixedDeltaTime;
    private float dyingElapsed;

    public GameState State { get; private set; }

    public bool IsPlaying => State == GameState.Playing;
    public bool CanSimulateWorld => IsPlaying || State == GameState.Dying;

    private void Awake()
    {       
        health = GetComponent<Health>();
        baseFixedDeltaTime = Time.fixedDeltaTime;
        SetState(GameState.Playing);
    }

    private void Update()
    {
        if(State != GameState.Dying)
        {
            return;
        }

        dyingElapsed += Time.unscaledDeltaTime;

        if(dyingElapsed >= dyingDuration)
        {
            SetState(GameState.GameOver);
        }   
    }

    private void OnEnable()
    {
        health.Died += HandleDeath;

        if(health.IsDead)
        {
            HandleDeath();
        }
    }

    private void OnDisable()
    {
        if(health != null)
        {
            health.Died -= HandleDeath;
        }
    }

    private void OnDestroy()
    {
        // 씬 이동 시 다음 씬 시간이 멈춰 있지 않도록 복원
        Time.timeScale = 1f;
        Time.fixedDeltaTime = baseFixedDeltaTime;
    }

    public bool TryBeginUpgradeSelection()
    {
        if(!IsPlaying || health.IsDead)
        {
            return false;
        }

        SetState(GameState.UpgradeSelection);
        return true;
    }

    public void EndUpgradeSelection()
    {
        if(State != GameState.UpgradeSelection)
        {
            return;
        }

        if(health.IsDead)
        {
            HandleDeath();
            return;
        }

        SetState(GameState.Playing);
    }

    private void HandleDeath()
    {
        if(State == GameState.GameOver || State == GameState.Dying)
        {
            return;
        }

        dyingElapsed = 0f;
        SetState(GameState.Dying);
    }

    private void SetState(GameState state)
    {
        State = state;
        if(State==GameState.Playing)
        {
            Time.timeScale = 1f;
        }
        else if(State==GameState.Dying)
        {
            Time.timeScale = deathTimeScale;
        }
        else {
            Time.timeScale = 0f;
        }

        if (State == GameState.Dying)
        {
            Time.fixedDeltaTime = baseFixedDeltaTime * deathTimeScale;
        } 
        else
        {
            Time.fixedDeltaTime = baseFixedDeltaTime;
        }
    }
}
