using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienSpawner : MonoBehaviour
{
    public List<GameObject> alienPrefabs;
    public List<SpawnPoint> spawnPoints;

    public float timeBetweenWaves = 10f;
    public int aliensPerWave = 3;
    public float spawnInterval = 1f;

    private int wave = 0;

    private float speedMultiplier = 1f;
    public float speedIncrease = 1f;
    public float interval = 3f;

    void Start()
    {
        StartCoroutine(SpawnWaves());
        StartCoroutine(IncreaseSpeedOverTime());
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            wave++;
            GameManager.Instance.UpdateWaveUI(wave);
            
            int count = aliensPerWave + wave;
            for (int i = 0; i < count; i++)
            {
                SpawnAlien();
                yield return new WaitForSeconds(spawnInterval);
            }
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnAlien()
    {
        if (alienPrefabs.Count == 0 || spawnPoints.Count == 0)
        {
            Debug.LogError("AlienSpawner: alienPrefabs หรือ spawnPoints ยังว่างอยู่!");
            return;
        }

        int randomPrefab = Random.Range(0, alienPrefabs.Count);
        int randomSpawn = Random.Range(0, spawnPoints.Count);

        GameObject alien = Instantiate(alienPrefabs[randomPrefab], spawnPoints[randomSpawn].transform);
        spawnPoints[randomSpawn].aliens.Add(alien);

        AlienController controller = alien.GetComponent<AlienController>();
        controller.Speed *= speedMultiplier;
    }
    IEnumerator IncreaseSpeedOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            speedMultiplier += speedIncrease;
            Debug.Log("Speed multiplier: " + speedMultiplier);
        }
    }
}


