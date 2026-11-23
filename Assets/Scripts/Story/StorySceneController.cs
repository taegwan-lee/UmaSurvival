using UnityEngine;

public class StorySceneController : MonoBehaviour
{
    [SerializeField] private DialogueController dialogue;
    [SerializeField] private StorySequence prologue;

    private void OnEnable()
    {
        if (dialogue != null)
        {
            dialogue.Finished += HandleFinished;
        }
    }

    private void Start()
    {
        if (dialogue == null || !dialogue.TryPlay(prologue))
        {
            Debug.LogWarning("프롤로그 대본과 대화 UI 연결을 확인해주세요.", this);
        }
    }

    private void OnDisable()
    {
        if (dialogue != null)
        {
            dialogue.Finished -= HandleFinished;
        }
    }

    private void HandleFinished()
    {
        Debug.Log("프롤로그 대화 종료", this);
    }
}
