using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private UnityEngine.UI.Image hpFill;
    [SerializeField] private TMPro.TextMeshProUGUI hpValue;
    [SerializeField] private Sprite dangerTrack;

    private UnityEngine.UI.Image hpTrack;
    private Sprite normalTrack;

    private int displayedHealth = -1;
    private int displayedMaxHealth = -1;

    private void Awake()
    {
        hpTrack = GetComponent<UnityEngine.UI.Image>();
        normalTrack = hpTrack.sprite;
    }

    private void Update()
    {
        if (playerHealth == null || hpFill == null || hpValue == null)
        {
            return;
        }

        int current = playerHealth.CurrentHealth;
        int maximum = playerHealth.MaxHealth;

        if (current == displayedHealth && maximum == displayedMaxHealth)
        {
            return;
        }

        float ratio = Mathf.Clamp01((float)current / Mathf.Max(1, maximum));
        hpFill.fillAmount = ratio;
        hpValue.text = $"HP {current} / {maximum}";

        if (dangerTrack != null)
        {
            hpTrack.sprite = ratio <= 0.25f ? dangerTrack : normalTrack;
        }

        displayedHealth = current;
        displayedMaxHealth = maximum;
    }
}
