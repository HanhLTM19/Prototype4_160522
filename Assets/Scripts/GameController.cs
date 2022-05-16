using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;

    float spawnRange = 9;

    //float startDelay = 2.0f;
    //float repeatRate = 3.0f;
    private int enemyCount;

    private int waveNum = 1;

    private bool m_isGameOver;
    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemy(waveNum);
        Instantiate(powerupPrefab, GetSpawnPosition(), powerupPrefab.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameOver())
        {
            Debug.Log("Game Over");
            return;
        }    
        enemyCount = FindObjectsOfType<Enemy>().Length;

        if (enemyCount == 0)
        {
            SpawnEnemy(++waveNum);
            Instantiate(powerupPrefab, GetSpawnPosition(), powerupPrefab.transform.rotation);
        }
    }
    void SpawnEnemy(int numSpawn)
    {
        for (int i = 0; i < numSpawn; i++)
        {
            Instantiate(enemyPrefab, GetSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }
    public Vector3 GetSpawnPosition ()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0.0f, spawnPosZ);
        return randomPos;
    }
    public void SetIsGameOver(bool state)
    {
        m_isGameOver = state;
    }
    public bool IsGameOver()
    {
        return m_isGameOver;
    }
}
