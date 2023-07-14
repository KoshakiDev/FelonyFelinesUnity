using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class WaveManager : MonoBehaviour
{

    public GameObject enemySpawners;


    public int currentWave = 1;

    List<GameObject> currentEnemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> currentEnemies = new List<GameObject>();
        UpdateWave();
    }

    // Update is called once per frame
    void Update()
    {
        
        int enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if(enemiesLeft == 0)
        {
            UpdateWave();
            currentWave += 1;
        }
        
    }

    void UpdateWave()
    {
        foreach (EnemySpawner enemySpawner in enemySpawners.GetComponentsInChildren<EnemySpawner>())
        {
            enemySpawner.SpawnEnemies(currentWave, this);
        }
    }

    public void AddEnemyToList(GameObject newEnemy)
    {
        currentEnemies.Add(newEnemy);
    }

    public void RemoveEnemyFromList(GameObject oldEnemy)
    {
        currentEnemies.Remove(oldEnemy);
    }
    
}
