using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(UnityEngine.UI.Button))]
public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text titleText;
    [SerializeField] private TMPro.TMP_Text descriptionText;

    [SerializeField] private UnityEngine.UI.Image cardAccent;
    [SerializeField] private UnityEngine.UI.Image badgeAccent;
    [SerializeField] private Color[] accentColors =
    {
        new Color32(105, 165, 72, 255),
        new Color32(204, 151, 43, 255),
        new Color32(213, 108, 149, 255)
    };

    private UnityEngine.UI.Button button;
    private LevelUpController levelUpController;

    private int offerId;
    private int choiceIndex;

    public bool IsConfigured => titleText != null && descriptionText != null;

    private void Awake()
    {
        button = GetComponent<UnityEngine.UI.Button>();
        button.interactable = false;
    }

    private void OnEnable()
    {
        button.onClick.AddListener(HandleClick);
    }

    private void OnDisable()
    {
        if(button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }
    }

    // 카드에 강화 연결
    public void Bind(
        UpgradeDefinition upgrade,
        LevelUpController controller,
        int offerId,
        int choiceIndex
    )
    {
        levelUpController = controller;
        this.offerId = offerId;
        this.choiceIndex = choiceIndex;

        titleText.text = upgrade.DisplayName;
        descriptionText.text = upgrade.Description;
        ApplyAccent(choiceIndex);

        button.interactable = true;
    }

    private void ApplyAccent(int index)
    {
        if (accentColors == null || accentColors.Length == 0)
        {
            return;
        }

        Color accent = accentColors[Mathf.Max(0, index) % accentColors.Length];
        if (cardAccent != null)
        {
            cardAccent.color = accent;
        }
        if (badgeAccent != null)
        {
            badgeAccent.color = accent;
        }
    }

    private void HandleClick()
    {
        if(!gameObject.activeInHierarchy ||
        !button.interactable ||
        levelUpController == null)
        {
            return;
        }

        button.interactable = false;

        bool applied = levelUpController.TrySelect(
            offerId,
            choiceIndex
        );

        if(!applied &&
        gameObject.activeInHierarchy &&
        levelUpController.IsChoosing &&
        levelUpController.CurrentOfferId == offerId)
        {
            button.interactable = true;
        }
    }
}
