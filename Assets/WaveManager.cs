using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public GameObject enemySpawners;


    // Start is called before the first frame update
    void Start()
    {
        UpdateWave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateWave()
    {
        foreach (EnemySpawner enemySpawner in enemySpawners.GetComponentsInChildren<EnemySpawner>())
        {
            enemySpawner.SpawnEnemies(1);
        }
    }
    
}
