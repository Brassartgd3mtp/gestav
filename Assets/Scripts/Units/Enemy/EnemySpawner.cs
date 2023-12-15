using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyToSpawn;
    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float maxSpawnInterval = 20f;
    [SerializeField] private float spawnInterval;
    [SerializeField] private GameObject detectionArea;
    private float maxSpawnDistance;
    public int unitsInArea;
    private int maxEnemiesAtSameTime = 4;
    public List<GameObject> Enemies = new List<GameObject>();


    private void Awake()
    {
        unitsInArea = 0;
        spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        maxSpawnDistance = detectionArea.GetComponent<SphereCollider>().radius / 2;
        spawnInterval = 6f;
    }
    private void Update()
    {
        if (unitsInArea > 0 && Enemies.Count < maxEnemiesAtSameTime)
        {
            if (spawnInterval < 0)
            {
                Vector3 _spawnPosition = new Vector3(transform.position.x + Random.Range(0, maxSpawnDistance), transform.position.y,transform.position.z + Random.Range(0, maxSpawnDistance));
                GameObject enemy = Instantiate(enemyToSpawn, _spawnPosition, Quaternion.identity);
                Enemies.Add(enemy);
                enemy.GetComponent<EnemyManager>().Spawner = this;
                spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            }
            else
            {
                spawnInterval -= Time.deltaTime;
            }
        }

    }
}