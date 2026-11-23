using System;
using UnityEngine;

public enum SpeakerSide
{
    Left,
    Right,
    Narrator
}

public enum PortraitExit
{
    None,
    Left,
    Right,
    Both
}

[Serializable]
public struct DialogueLine
{
    [SerializeField] private SpeakerSide speaker;
    [SerializeField, TextArea(2,4)] private string text;
    [SerializeField] private Sprite expression;
    [SerializeField, Tooltip("Plays once when this line begins.")] private AudioClip audioClip;
    [SerializeField, Tooltip("Choose who leaves when this line begins.")] private PortraitExit exitPortrait;

    public SpeakerSide Speaker => speaker;
    public string Text => text;
    public Sprite Expression => expression;
    public AudioClip AudioClip => audioClip;
    public PortraitExit ExitPortrait => exitPortrait;
}

[CreateAssetMenu(menuName = "UmaSurvival/Story Sequence")]
public sealed class StorySequence : ScriptableObject
{
    [SerializeField] private Sprite background;
    [SerializeField] private string leftName;
    [SerializeField] private Sprite leftPortrait;
    [SerializeField] private string rightName;
    [SerializeField] private Sprite rightPortrait;
    [SerializeField] private DialogueLine[] lines = Array.Empty<DialogueLine>();

    public Sprite Background => background;
    public Sprite LeftPortrait => leftPortrait;
    public Sprite RightPortrait => rightPortrait;
    public int LineCount => lines.Length;

    public DialogueLine GetLine(int index) => lines[index];

    public string GetSpeakerName(SpeakerSide side)
    {
        return side switch
        {
            SpeakerSide.Left => leftName,
            SpeakerSide.Right => rightName,
            _ => string.Empty
        };
    }
}
