using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private GameFlowController gameFlow;
    [SerializeField] private SurvivalTimer gameTimer;

    [SerializeField] private EnemyController fastEnemyPrefab;
    [SerializeField] private float fastEnemyStartTime = 30f;
    [SerializeField] private int fastEnemyProbability= 30;

    [SerializeField] private EnemyController tankEnemyPrefab;
    [SerializeField] private float tankEnemyStartTime = 60f;
    [SerializeField] private int tankEnemyProbability = 20;

    [SerializeField, Min(0.1f)]
    private float spawnInterval = 1.5f;

    [SerializeField, Min(0f)]
    private float spawnMargin = 1f;

    private float spawnTimer;

    private void Update()
    {   
        if(gameFlow == null || !gameFlow.IsPlaying)
        {
            return;
        }

        if(enemyPrefab == null || player == null || gameCamera == null)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;

        if(spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = GetSpawnInterval();
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetSpawnPosition();
        EnemyController selectedPrefab = ChooseEnemyPrefab();

        EnemyController enemy = Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity
        );

        enemy.Initialize(player);
    }

    private EnemyController ChooseEnemyPrefab()
    {
        float elapsed = gameTimer.ElapsedSeconds;
        int roll = Random.Range(0,100);
        int fastCutoff = 0;

        if(fastEnemyPrefab != null && elapsed >= fastEnemyStartTime)
        {
            fastCutoff = fastEnemyProbability;

            if(roll < fastCutoff)
            {
                return fastEnemyPrefab;
            }
        }

        if(tankEnemyPrefab!= null && 
        elapsed >= tankEnemyStartTime && 
        roll < fastCutoff + tankEnemyProbability)
        {
            return tankEnemyPrefab;
        }

        return enemyPrefab;
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 cameraPosition = gameCamera.transform.position;

        float halfHeight = gameCamera.orthographicSize;
        float halfWidth = halfHeight * gameCamera.aspect;

        float minX = cameraPosition.x - halfWidth - spawnMargin;
        float maxX = cameraPosition.x + halfWidth + spawnMargin;
        float minY = cameraPosition.y - halfHeight - spawnMargin;
        float maxY = cameraPosition.y + halfHeight + spawnMargin;

        int side = Random.Range(0,4);

        switch(side)
        {
            case 0: // 왼쪽
            return new Vector3(
                minX, Random.Range(minY, maxY), player.position.z
            );

            case 1: // 오른쪽
            return new Vector3(
                maxX, Random.Range(minY, maxY), player.position.z
                );

            case 2: // 아래쪽
                return new Vector3(
                    Random.Range(minX, maxX), minY, player.position.z
                );

            default: // 위쪽
                return new Vector3(
                    Random.Range(minX, maxX), maxY, player.position.z
                );
        }
    }

    private float GetSpawnInterval()
    {
        if(gameTimer==null)
        {
            return spawnInterval;
        }

        float elapsed = gameTimer.ElapsedSeconds;

        if(elapsed >= 60f)
        {
            return 0.7f;
        }

        if(elapsed >= 30f)
        {
            return 1f;
        }

        return spawnInterval;
    }
}
