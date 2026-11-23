using UnityEngine;

public class SurvivalTimerUI : MonoBehaviour
{   
    [SerializeField]
    private SurvivalTimer survivalTimer;

    private TMPro.TextMeshProUGUI timerText;

    private float displayedSeconds;

    private void Awake()
    {
        timerText = GetComponent<TMPro.TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        displayedSeconds = -1;
    }

    private void Update()
    {
        if(survivalTimer == null)
        {
             return;
        }

        int totalSeconds = Mathf.FloorToInt(
            survivalTimer.ElapsedSeconds
        );

        if(totalSeconds == displayedSeconds)
        {
            return;
        }

        displayedSeconds = totalSeconds;

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
