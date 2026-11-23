using UnityEngine;

public class ExperienceCarrot : MonoBehaviour
{
    [SerializeField]private int experienceAmonut = 1;

    private bool isCollected;

    private void OnTriggerEnter2D(Collider2D others)
    {   
        if(isCollected)
        {
            return;
        }

        PlayerExperience playerExperience = 
            others.GetComponentInParent<PlayerExperience>();

        if(playerExperience == null || !playerExperience.CompareTag("Player"))
        {
            return;
        }

        isCollected = true;

        playerExperience.AddExperience(experienceAmonut);

        Destroy(gameObject);
    } 

    
}
