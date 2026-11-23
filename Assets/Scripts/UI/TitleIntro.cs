using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class TitleIntro : MonoBehaviour
{
    private const float Duration = 0.9f;
    private const float FirstLogoReveal = 0.45f;
    private const float MaxFrameStep = 0.05f;

    [SerializeField] private Image titleLogo;
    [SerializeField] private CanvasGroup menuGroup;
    [SerializeField] private TitleRevealEffect revealEffect;

    private float elapsed;
    private bool isPlaying;
    private Color logoColor;
    private Vector3 logoScale;
    private Image.Type logoType;
    private Image.FillMethod logoFillMethod;
    private int logoFillOrigin;
    private float logoFillAmount;
    private float menuAlpha;
    private bool menuInteractable;
    private bool menuBlocksRaycasts;

    private void OnEnable()
    {
        if (titleLogo == null || menuGroup == null || revealEffect == null)
        {
            return;
        }

        logoColor = titleLogo.color;
        logoScale = titleLogo.rectTransform.localScale;
        logoType = titleLogo.type;
        logoFillMethod = titleLogo.fillMethod;
        logoFillOrigin = titleLogo.fillOrigin;
        logoFillAmount = titleLogo.fillAmount;
        menuAlpha = menuGroup.alpha;
        menuInteractable = menuGroup.interactable;
        menuBlocksRaycasts = menuGroup.blocksRaycasts;

        elapsed = 0f;
        isPlaying = true;
        titleLogo.type = Image.Type.Filled;
        titleLogo.fillMethod = Image.FillMethod.Vertical;
        titleLogo.fillOrigin = (int)Image.OriginVertical.Top;
        menuGroup.interactable = false;
        menuGroup.blocksRaycasts = false;
        UpdatePresentation();
        revealEffect.enabled = true;
    }

    private void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        // Preserve the reveal beats when loading the scene produces a long frame.
        elapsed += Mathf.Min(Time.unscaledDeltaTime, MaxFrameStep);
        if (elapsed >= Duration)
        {
            enabled = false;
            return;
        }

        UpdatePresentation();
    }

    private void UpdatePresentation()
    {
        float upperReveal = SmoothProgress(0.19f, 0.38f);
        float lowerReveal = SmoothProgress(0.42f, 0.61f);
        titleLogo.fillAmount = FirstLogoReveal * upperReveal + (1f - FirstLogoReveal) * lowerReveal;

        Color color = logoColor;
        color.a *= SmoothProgress(0.19f, 0.30f);
        titleLogo.color = color;

        float grow = EaseOutProgress(0.19f, 0.43f);
        float settle = SmoothProgress(0.43f, 0.66f);
        float scale = Mathf.Lerp(0.92f, 1.055f, grow) - 0.055f * settle;
        titleLogo.rectTransform.localScale = logoScale * scale;

        menuGroup.alpha = menuAlpha * SmoothProgress(0.63f, 0.86f);
        Vector2 center = revealEffect.rectTransform.InverseTransformPoint(titleLogo.rectTransform.position);
        revealEffect.SetFrame(elapsed, center);
    }

    private float SmoothProgress(float start, float end)
    {
        return Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(start, end, elapsed));
    }

    private float EaseOutProgress(float start, float end)
    {
        float remaining = 1f - Mathf.InverseLerp(start, end, elapsed);
        return 1f - remaining * remaining * remaining;
    }

    private void OnDisable()
    {
        if (!isPlaying)
        {
            return;
        }

        isPlaying = false;
        if (titleLogo != null)
        {
            titleLogo.type = logoType;
            titleLogo.fillMethod = logoFillMethod;
            titleLogo.fillOrigin = logoFillOrigin;
            titleLogo.fillAmount = logoFillAmount;
            titleLogo.color = logoColor;
            titleLogo.rectTransform.localScale = logoScale;
        }

        if (menuGroup != null)
        {
            menuGroup.alpha = menuAlpha;
            menuGroup.interactable = menuInteractable;
            menuGroup.blocksRaycasts = menuBlocksRaycasts;
        }

        if (revealEffect != null)
        {
            revealEffect.enabled = false;
        }
    }
}
