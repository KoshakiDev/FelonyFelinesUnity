using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public GameObject enemySpawners;


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
        
        /*
        You could technically take all of the newly spawned enemies and parent them to an empty and just keep track of that empty's count
        */
        int enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;
        
        
        if(enemiesLeft == 0)
        {
            UpdateWave();
        }
        
    }

    void UpdateWave()
    {
        foreach (EnemySpawner enemySpawner in enemySpawners.GetComponentsInChildren<EnemySpawner>())
        {
            enemySpawner.SpawnEnemies(1, this);
        }
    }

    public void AddEnemyToList(GameObject newEnemy)
    {
        currentEnemies.Add(newEnemy);
    }
    
}
