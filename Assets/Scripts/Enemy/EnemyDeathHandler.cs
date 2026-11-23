using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    [SerializeField] ExperienceCarrot experienceCarrotPrefab;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.Died += HandleDeath;
    }

    private void OnDisable()
    {   
        if(health==null)
        {
            return;
        }
        health.Died -= HandleDeath;
    }

    private void HandleDeath()
    {   
        if(experienceCarrotPrefab!=null)
        {
            Instantiate(
            experienceCarrotPrefab,
            transform.position,
            Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}
