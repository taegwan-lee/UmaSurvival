using UnityEngine;

public class LevelExperienceUI : MonoBehaviour
{
    [SerializeField] private PlayerExperience playerExperience;
    [SerializeField] private RectTransform raceTrack;
    [SerializeField] private RectTransform xpFill;
    [SerializeField] private RectTransform carrotMarker;
   
    private int displayedLevel = -1;
    private int displayedExperience = -1;
    private int displayedRequiredExperience = -1;
    private float displayedTrackWidth = -1f;

    private void Awake()
    {
        
    }

    private void Update()
    {
        if (playerExperience == null || raceTrack == null || xpFill == null ||
            carrotMarker == null)
        {
            return;
        }

        int level = playerExperience.Level;
        int experience = playerExperience.CurrentExperience;
        int required = playerExperience.RequiredExperience;
        float trackWidth = raceTrack.rect.width;

        if (level == displayedLevel && experience == displayedExperience &&
            required == displayedRequiredExperience && trackWidth == displayedTrackWidth)
        {
            return;
        }

        float progress = Mathf.Clamp01((float)experience / Mathf.Max(1, required));
        float filledWidth = trackWidth * progress;

        xpFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, filledWidth);
        carrotMarker.anchoredPosition = new Vector2(filledWidth, 0f);

        displayedLevel = level;
        displayedExperience = experience;
        displayedRequiredExperience = required;
        displayedTrackWidth = trackWidth;
    }
}
