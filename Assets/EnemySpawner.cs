using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains;
using MoreMountains.Tools;


public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> ObjectsToInstantiate;

    public MMSpawnAroundProperties SpawnProperties;

    protected GameObject _gameObject;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void SpawnEnemies(int amount, WaveManager waveManager)
    {
        for (int i = 0; i < amount; i++)
        {
            int index = UnityEngine.Random.Range(0, ObjectsToInstantiate.Count);
            spawnSingleEnemy(ObjectsToInstantiate[index], waveManager);
            
        }
    }

    void spawnSingleEnemy(GameObject enemy, WaveManager waveManager)
    {

        _gameObject = Instantiate(enemy);
        SceneManager.MoveGameObjectToScene(_gameObject, this.gameObject.scene);
        MoreMountains.Tools.MMSpawnAround.ApplySpawnAroundProperties(_gameObject, SpawnProperties, this.transform.position);

        waveManager.AddEnemyToList(_gameObject);
    }
}

