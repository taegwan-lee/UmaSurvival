using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;

public class UpgradeSelectionUI : MonoBehaviour
{
    [SerializeField] private LevelUpController levelUpController;
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardRoot;
    [SerializeField] private UpgradeCardUI cardPrefab;

    [Header("Open Animation")]
    [SerializeField, Min(0f)] private float openDuration = 0.32f;
    [SerializeField, Range(0f, 1f)] private float initialOverlap = 0.65f;
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip openCardsSE;
    

    private readonly List<UpgradeCardUI> cards =
        new List<UpgradeCardUI>();

    private CanvasGroup panelGroup;
    private UnityEngine.UI.HorizontalLayoutGroup cardLayout;
    private Coroutine opening;
    private float restSpacing;
    private Vector3 restScale;

    private void Awake()
    {
        if (levelUpController == null ||
            panel == null ||
            cardRoot == null ||
            cardPrefab == null ||
            !cardPrefab.IsConfigured)
        {
            Debug.LogError(
                "강화 UI 참조와 카드 프리팹의 텍스트 연결을 확인해주세요.",
                this
            );

            enabled = false;
            return;
        }

        panelGroup = panel.GetComponent<CanvasGroup>();
        if (panelGroup == null)
        {
            panelGroup = panel.AddComponent<CanvasGroup>();
        }

        cardLayout = cardRoot.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        if (cardLayout != null)
        {
            restSpacing = cardLayout.spacing;
        }
        restScale = cardRoot.localScale;

        panel.SetActive(false);
    }

    private void OnEnable()
    {
        levelUpController.SelectionOpened += Show;
        levelUpController.SelectionClosed += Hide;

        // UI가 켜지기 전에 선택이 시작된 경우도 반영
        if (levelUpController.IsChoosing)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void OnDisable()
    {
        if (levelUpController != null)
        {
            levelUpController.SelectionOpened -= Show;
            levelUpController.SelectionClosed -= Hide;
        }

        Hide();
    }

    private void Show()
    {
        ClearCards();

        if (!levelUpController.IsChoosing)
        {
            panel.SetActive(false);
            return;
        }

        // 카드 초기화 전에 부모 패널부터 활성화
        panel.SetActive(true);

        int offerId = levelUpController.CurrentOfferId;
        IReadOnlyList<UpgradeDefinition> choices =
            levelUpController.CurrentChoices;

        for (int i = 0; i < choices.Count; i++)
        {
            UpgradeCardUI card = Instantiate(
                cardPrefab,
                cardRoot
            );

            card.gameObject.SetActive(true);

            card.Bind(
                choices[i],
                levelUpController,
                offerId,
                i
            );

            cards.Add(card);
        }

        if (openDuration > 0f)
        {   
            uiAudioSource.PlayOneShot(openCardsSE);
            opening = StartCoroutine(OpenRoutine(openDuration));
        }
    }

    private IEnumerator OpenRoutine(float duration)
    {
        float width = ((RectTransform)cardPrefab.transform).rect.width;
        float startSpacing = restSpacing - width * initialOverlap;

        panelGroup.interactable = false;
        ApplyOpeningPose(0f, startSpacing);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            yield return null;
            elapsed += Time.unscaledDeltaTime;
            ApplyOpeningPose(Mathf.Clamp01(elapsed / duration), startSpacing);
        }

        panelGroup.interactable = true;
        opening = null;
    }

    private void ApplyOpeningPose(float progress, float startSpacing)
    {
        float remaining = 1f - progress;
        float spread = 1f - remaining * remaining * remaining;
        if (cardLayout != null)
        {
            cardLayout.spacing = Mathf.Lerp(startSpacing, restSpacing, spread);
        }

        cardRoot.localScale = restScale * Mathf.Lerp(0.88f, 1f, spread);
        panelGroup.alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress / 0.75f));
    }

    private void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        ClearCards();
    }

    private void ClearCards()
    {
        if (opening != null)
        {
            StopCoroutine(opening);
            opening = null;
            ApplyOpeningPose(1f, restSpacing);
            panelGroup.interactable = true;
        }

        foreach (UpgradeCardUI card in cards)
        {
            if (card == null)
            {
                continue;
            }

            card.gameObject.SetActive(false);
            Destroy(card.gameObject);
        }

        cards.Clear();
    }
}
