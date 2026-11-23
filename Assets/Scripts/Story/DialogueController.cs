using System;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private DialogueView view;

    private StorySequence activeStory;
    private int lineIndex;

    public event Action Finished;

    public bool TryPlay(StorySequence story)
    {
        if (activeStory != null || story == null || story.LineCount == 0 ||
            view == null || !view.isActiveAndEnabled || !view.IsReady)
        {
            return false;
        }

        activeStory = story;
        lineIndex = 0;
        view.Prepare(story);
        ShowCurrentLine();
        return true;
    }

    public void Next()
    {
        if (activeStory == null)
        {
            return;
        }

        if (view.RevealCurrentLine())
        {
            return;
        }

        lineIndex++;

        if (lineIndex >= activeStory.LineCount)
        {
            activeStory = null;
            view.StopLineAudio();
            Finished?.Invoke();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        DialogueLine line = activeStory.GetLine(lineIndex);
        string displayName = activeStory.GetSpeakerName(line.Speaker);
        view.ShowLine(displayName, line);
    }
}
