using System.Collections;
using UnityEngine;

public class PopupTransition : MonoBehaviour
{
    [Header("UI references")]
    [SerializeField] private CanvasGroup rootGroup;
    [SerializeField] private UnityEngine.UI.RawImage backdrop;
    [SerializeField] private CanvasGroup backdropGroup;
    [SerializeField] private RectTransform popupPivot;
    [SerializeField] private RectTransform revealMask;
    [SerializeField] private CanvasGroup windowGroup;

    [Header("Background blur")]
    [SerializeField] private Shader blurShader;
    [SerializeField, Range(0.5f, 3f)] private float blurStrength = 1f;

    [Header("Animation")]
    [SerializeField, Min(0.01f)] private float openDuration = 0.25f;
    [SerializeField, Min(0.01f)] private float revealDuration = 0.14f;
    [SerializeField, Min(0.01f)] private float closeDuration = 0.15f;
    [SerializeField] private float closeDistance = 20f;
    [SerializeField] private AnimationCurve scaleCurve = new AnimationCurve(
        new Keyframe(0f, 0.72f, 0f, 1.3f),
        new Keyframe(0.6f, 1.018f, 0f, 0f),
        new Keyframe(1f, 1f, 0f, 0f)
    );

    private Material blurMaterial;
    private RenderTexture blurredBackground;
    private Vector2 popupPosition;
    private bool isAnimating;

    public bool IsReady { get; private set; }

    private void Awake()
    {
        if (rootGroup == null || backdrop == null || backdropGroup == null ||
            popupPivot == null || revealMask == null || windowGroup == null ||
            blurShader == null)
        {
            Debug.LogError("PopupTransition의 Inspector 연결을 확인해주세요.", this);
            enabled = false;
            return;
        }

        popupPosition = popupPivot.anchoredPosition;
        blurMaterial = new Material(blurShader);

        rootGroup.alpha = 0f;
        rootGroup.interactable = false;
        rootGroup.blocksRaycasts = false;
    }

    public void Open()
    {
        if (!isActiveAndEnabled || isAnimating || IsReady)
        {
            return;
        }

        isAnimating = true;
        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        rootGroup.transform.SetAsLastSibling();
        rootGroup.alpha = 0f;
        rootGroup.interactable = false;
        rootGroup.blocksRaycasts = true;

        // 팝업을 숨긴 상태에서 게임 화면과 HUD를 함께 캡처합니다.
        yield return new WaitForEndOfFrame();

        try
        {
            CaptureBackground();
        }
        catch (System.Exception exception)
        {
            ReleaseBackground();
            backdrop.color = new Color(1f, 1f, 1f, 0.35f);
            Debug.LogWarning("Popup background capture failed: " + exception.Message, this);
        }

        popupPivot.anchoredPosition = popupPosition;
        popupPivot.localScale = Vector3.one;
        float diameter = GetMaskDiameter();

        backdropGroup.alpha = 0f;
        windowGroup.alpha = 0f;
        popupPivot.localScale = Vector3.one * scaleCurve.Evaluate(0f);
        revealMask.sizeDelta = Vector2.zero;
        rootGroup.alpha = 1f;

        float elapsed = 0f;
        float duration = Mathf.Max(openDuration, revealDuration, 0.12f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / openDuration);
            float reveal = EaseOutCubic(Mathf.Clamp01(elapsed / revealDuration));

            popupPivot.localScale = Vector3.one * scaleCurve.Evaluate(progress);
            revealMask.sizeDelta = Vector2.one * diameter * reveal;
            backdropGroup.alpha = EaseOutCubic(Mathf.Clamp01(elapsed / 0.12f));
            windowGroup.alpha = EaseOutCubic(Mathf.Clamp01(elapsed / 0.1f));

            yield return null;
        }

        popupPivot.localScale = Vector3.one;
        revealMask.sizeDelta = Vector2.one * diameter;
        backdropGroup.alpha = 1f;
        windowGroup.alpha = 1f;
        rootGroup.interactable = true;
        isAnimating = false;
        IsReady = true;
    }

    public IEnumerator Close()
    {
        if (!IsReady || isAnimating)
        {
            yield break;
        }

        IsReady = false;
        isAnimating = true;
        rootGroup.interactable = false;

        float elapsed = 0f;

        while (elapsed < closeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / closeDuration);

            popupPivot.anchoredPosition =
                popupPosition + Vector2.down * closeDistance * progress;
            windowGroup.alpha = 1f - progress * progress;
            backdropGroup.alpha = 1f - progress;

            yield return null;
        }

        rootGroup.alpha = 0f;
        rootGroup.blocksRaycasts = false;
        popupPivot.anchoredPosition = popupPosition;
        isAnimating = false;
        ReleaseBackground();
    }

    private float GetMaskDiameter()
    {
        RectTransform window = (RectTransform)windowGroup.transform;
        Vector2 center = window.InverseTransformPoint(revealMask.position);
        Rect bounds = window.rect;

        float x = Mathf.Max(
            Mathf.Abs(bounds.xMin - center.x),
            Mathf.Abs(bounds.xMax - center.x)
        );
        float y = Mathf.Max(
            Mathf.Abs(bounds.yMin - center.y),
            Mathf.Abs(bounds.yMax - center.y)
        );

        return 2f * (new Vector2(x, y).magnitude + 16f);
    }

    private void CaptureBackground()
    {
        ReleaseBackground();

        Texture2D capture = null;
        RenderTexture temporary = null;
        RenderTexture previous = RenderTexture.active;
        bool previousSrgbWrite = GL.sRGBWrite;

        try
        {
            capture = ScreenCapture.CaptureScreenshotAsTexture();
            int width = Mathf.Max(1, capture.width / 4);
            int height = Mathf.Max(1, capture.height / 4);

            blurredBackground = new RenderTexture(width, height, 0);
            blurredBackground.filterMode = FilterMode.Bilinear;
            blurredBackground.wrapMode = TextureWrapMode.Clamp;
            blurredBackground.Create();

            temporary = RenderTexture.GetTemporary(width, height, 0);
            temporary.filterMode = FilterMode.Bilinear;
            temporary.wrapMode = TextureWrapMode.Clamp;

            GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
            Graphics.Blit(capture, blurredBackground);

            blurMaterial.SetVector("_BlurDirection", new Vector4(blurStrength, 0f, 0f, 0f));
            Graphics.Blit(blurredBackground, temporary, blurMaterial);

            blurMaterial.SetVector("_BlurDirection", new Vector4(0f, blurStrength, 0f, 0f));
            Graphics.Blit(temporary, blurredBackground, blurMaterial);

            backdrop.texture = blurredBackground;
            backdrop.color = Color.white;
        }
        finally
        {
            GL.sRGBWrite = previousSrgbWrite;
            RenderTexture.active = previous;

            if (temporary != null)
            {
                RenderTexture.ReleaseTemporary(temporary);
            }

            if (capture != null)
            {
                Destroy(capture);
            }
        }
    }

    private static float EaseOutCubic(float value)
    {
        float remaining = 1f - value;
        return 1f - remaining * remaining * remaining;
    }

    private void ReleaseBackground()
    {
        if (backdrop != null)
        {
            backdrop.texture = null;
        }

        if (blurredBackground != null)
        {
            blurredBackground.Release();
            Destroy(blurredBackground);
            blurredBackground = null;
        }
    }

    private void OnDestroy()
    {
        ReleaseBackground();

        if (blurMaterial != null)
        {
            Destroy(blurMaterial);
        }
    }
}
