using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image background;
    [SerializeField] private UnityEngine.UI.Image leftPortrait;
    [SerializeField] private UnityEngine.UI.Image rightPortrait;
    [SerializeField] private CanvasGroup leftPortraitGroup;
    [SerializeField] private CanvasGroup rightPortraitGroup;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private AudioSource lineAudioSource;

    [Header("Speaker Colors")]
    [SerializeField] private UnityEngine.UI.Image dialogueCardImage;
    [SerializeField] private UnityEngine.UI.Image speakerTabImage;
    [SerializeField] private UnityEngine.UI.Image nextButtonImage;
    [SerializeField] private Color leftSpeakerAccent = new Color32(174, 52, 73, 255);
    [SerializeField] private Color rightSpeakerAccent = new Color32(38, 107, 68, 255);
    [SerializeField] private Color narratorAccent = new Color32(61, 80, 100, 255);
    [SerializeField, Min(0f)] private float narrationTopPadding = 42f;

    [SerializeField, Min(1f)] private float charactersPerSecond = 38f;
    [SerializeField, Min(0.01f)] private float entranceDuration = 0.25f;
    [SerializeField, Min(0.01f)] private float focusDuration = 0.15f;
    [SerializeField, Min(0f)] private float entranceOffset = 112f;
    [SerializeField] private Color listenerTint = new Color(0.64f, 0.67f, 0.73f, 1f);

    private Vector2 leftSeat;
    private Vector2 rightSeat;
    private float dialogueTextTopOffset;
    private bool leftOnStage;
    private bool rightOnStage;
    private Coroutine leftMotion;
    private Coroutine rightMotion;
    private float visibleCharacters;
    private int characterCount;

    public bool IsReady =>
        speakerName != null && dialogueText != null &&
        leftPortrait != null && rightPortrait != null &&
        leftPortraitGroup != null && rightPortraitGroup != null;

    private void Awake()
    {
        if (leftPortrait != null)
        {
            leftSeat = leftPortrait.rectTransform.anchoredPosition;
        }
        if (rightPortrait != null)
        {
            rightSeat = rightPortrait.rectTransform.anchoredPosition;
        }
        if (dialogueText != null)
        {
            dialogueTextTopOffset = dialogueText.rectTransform.offsetMax.y;
        }
    }

    private void OnDisable()
    {
        StopLineAudio();
    }

    public void Prepare(StorySequence story)
    {
        StopAllCoroutines();
        StopLineAudio();
        leftMotion = null;
        rightMotion = null;
        leftOnStage = false;
        rightOnStage = false;
        visibleCharacters = 0f;
        characterCount = 0;

        SetImage(background, story.Background);
        SetImage(leftPortrait, story.LeftPortrait);
        SetImage(rightPortrait, story.RightPortrait);
        leftPortrait.rectTransform.anchoredPosition = leftSeat;
        rightPortrait.rectTransform.anchoredPosition = rightSeat;
        leftPortrait.color = Color.white;
        rightPortrait.color = Color.white;
        leftPortraitGroup.alpha = 0f;
        rightPortraitGroup.alpha = 0f;
    }

    public void ShowLine(string displayName, DialogueLine line)
    {
        bool isNarration = line.Speaker == SpeakerSide.Narrator;
        speakerName.text = displayName;
        speakerName.gameObject.SetActive(!isNarration);
        if (speakerTabImage != null)
        {
            speakerTabImage.enabled = !isNarration;
        }

        RectTransform textRect = dialogueText.rectTransform;
        Vector2 upperOffset = textRect.offsetMax;
        upperOffset.y = isNarration ? -narrationTopPadding : dialogueTextTopOffset;
        textRect.offsetMax = upperOffset;

        dialogueText.text = line.Text ?? string.Empty;
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate();
        characterCount = dialogueText.textInfo.characterCount;
        visibleCharacters = 0f;

        if (!isNarration && line.Expression != null)
        {
            var portrait = line.Speaker == SpeakerSide.Left
                ? leftPortrait
                : rightPortrait;

            SetImage(portrait, line.Expression);
        }

        Color accent = line.Speaker switch
        {
            SpeakerSide.Left => leftSpeakerAccent,
            SpeakerSide.Right => rightSpeakerAccent,
            _ => narratorAccent
        };
        ApplySpeakerColors(accent);
        UpdatePortraits(line.Speaker, line.ExitPortrait);
        PlayLineAudio(line.AudioClip);
    }

    public void StopLineAudio()
    {
        if (lineAudioSource != null)
        {
            lineAudioSource.Stop();
            lineAudioSource.clip = null;
        }
    }

    private void PlayLineAudio(AudioClip clip)
    {
        StopLineAudio();
        if (lineAudioSource != null && clip != null)
        {
            lineAudioSource.clip = clip;
            lineAudioSource.Play();
        }
    }

    public bool RevealCurrentLine()
    {
        if (visibleCharacters >= characterCount)
        {
            return false;
        }

        visibleCharacters = characterCount;
        dialogueText.maxVisibleCharacters = characterCount;
        return true;
    }

    private void Update()
    {
        if (visibleCharacters >= characterCount)
        {
            return;
        }

        visibleCharacters = Mathf.Min(characterCount, visibleCharacters + charactersPerSecond * Time.unscaledDeltaTime);
        dialogueText.maxVisibleCharacters = Mathf.FloorToInt(visibleCharacters);
    }

    private void UpdatePortraits(SpeakerSide speaker, PortraitExit exit)
    {
        bool leftSpeaking = speaker == SpeakerSide.Left;
        bool rightSpeaking = speaker == SpeakerSide.Right;
        bool exitLeft = exit == PortraitExit.Left || exit == PortraitExit.Both;
        bool exitRight = exit == PortraitExit.Right || exit == PortraitExit.Both;

        if (exitLeft && leftOnStage)
        {
            ExitPortrait(leftPortrait, leftPortraitGroup, -entranceOffset, ref leftMotion);
        }
        if (exitRight && rightOnStage)
        {
            ExitPortrait(rightPortrait, rightPortraitGroup, entranceOffset, ref rightMotion);
        }

        bool leftEntering = leftSpeaking && !exitLeft && !leftOnStage && leftPortrait.sprite != null;
        bool rightEntering = rightSpeaking && !exitRight && !rightOnStage && rightPortrait.sprite != null;
        leftOnStage = !exitLeft && (leftOnStage || leftEntering);
        rightOnStage = !exitRight && (rightOnStage || rightEntering);
        bool bothVisible = leftOnStage && rightOnStage;

        MovePortrait(leftPortrait, leftPortraitGroup, leftSeat, bothVisible, leftOnStage,
            leftEntering, leftSpeaking, -entranceOffset, ref leftMotion);
        MovePortrait(rightPortrait, rightPortraitGroup, rightSeat, bothVisible, rightOnStage,
            rightEntering, rightSpeaking, entranceOffset, ref rightMotion);
    }

    private void ApplySpeakerColors(Color accent)
    {
        if (dialogueCardImage != null)
        {
            Color paperColor = Color.Lerp(Color.white, accent, 0.065f);
            paperColor.a = dialogueCardImage.color.a;
            dialogueCardImage.color = paperColor;
        }

        if (speakerTabImage != null)
        {
            speakerTabImage.color = accent;
        }

        if (nextButtonImage != null)
        {
            nextButtonImage.color = accent;
        }
    }

    private void MovePortrait(UnityEngine.UI.Image portrait, CanvasGroup group, Vector2 seat, bool bothVisible,
        bool onStage, bool entering, bool isSpeaker, float offset, ref Coroutine motion)
    {
        if (!onStage)
        {
            return;
        }
        if (motion != null)
        {
            StopCoroutine(motion);
        }

        Vector2 destination = bothVisible ? seat : new Vector2(0f, seat.y);
        Color tint = isSpeaker ? Color.white : listenerTint;
        if (entering && group.alpha <= 0f)
        {
            portrait.rectTransform.anchoredPosition = destination + new Vector2(offset, 0f);
            portrait.color = tint;
        }

        motion = StartCoroutine(AnimatePortrait(portrait, group, destination, tint, 1f));
    }

    private void ExitPortrait(UnityEngine.UI.Image portrait, CanvasGroup group, float offset, ref Coroutine motion)
    {
        if (motion != null)
        {
            StopCoroutine(motion);
        }

        Vector2 destination = portrait.rectTransform.anchoredPosition + new Vector2(offset, 0f);
        motion = StartCoroutine(AnimatePortrait(portrait, group, destination, portrait.color, 0f));
    }

    private IEnumerator AnimatePortrait(UnityEngine.UI.Image portrait, CanvasGroup group, Vector2 destination, Color tint, float targetAlpha)
    {
        RectTransform rect = portrait.rectTransform;
        Vector2 startPosition = rect.anchoredPosition;
        Color startTint = portrait.color;
        float startAlpha = group.alpha;
        float duration = Mathf.Max(entranceDuration, focusDuration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            yield return null;
            elapsed += Time.unscaledDeltaTime;
            float movement = Mathf.SmoothStep(0f, 1f, elapsed / entranceDuration);
            float focus = Mathf.SmoothStep(0f, 1f, elapsed / focusDuration);
            rect.anchoredPosition = Vector2.Lerp(startPosition, destination, movement);
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, movement);
            portrait.color = Color.Lerp(startTint, tint, focus);
        }
    }

    private static void SetImage(UnityEngine.UI.Image image, Sprite sprite)
    {
        if (image == null)
        {
            return;
        }

        image.sprite = sprite;
        image.enabled = sprite != null;
    }
}
