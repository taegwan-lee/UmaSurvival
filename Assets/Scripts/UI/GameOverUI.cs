using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameFlowController gameFlow;
    [SerializeField] private PopupTransition popup;
    [SerializeField] private float gameOverDelay = 0.8f;

    private bool hasShown;
    private bool isRestarting;

    private void Awake()
    {
        if (gameFlow == null || popup == null)
        {
            Debug.LogError("GameOverUI의 Inspector 연결을 확인해주세요.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (hasShown || gameFlow.State != GameState.GameOver)
        {
            return;
        }

        hasShown = true;
        StartCoroutine(ShowAfterDelay());
    }

    public void RestartGame()
    {
        if (isRestarting || !popup.IsReady)
        {
            return;
        }

        isRestarting = true;
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return popup.Close();

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    [ContextMenu("Test Game Over")]
    private void TestGameOver()
    {
        if (!Application.isPlaying || gameFlow == null)
        {
            return;
        }

        Health health = gameFlow.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(health.MaxHealth);
        }
    }

    private IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);

        if(gameFlow.State == GameState.GameOver)
        {
            popup.Open();
        }
    }
}
