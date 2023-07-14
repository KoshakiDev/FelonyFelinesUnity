using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains;
using MoreMountains.Tools;

using MoreMountains.TopDownEngine;


public class EnemySpawner : MonoBehaviour
{

    /// the object pooler associated to this spawner
    public MMObjectPooler ObjectPooler { get; set; }

    public MMSpawnAroundProperties SpawnProperties;

    protected GameObject _gameObject;
    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        if (GetComponent<MMMultipleObjectPooler>() != null)
        {
            ObjectPooler = GetComponent<MMMultipleObjectPooler>();
        }
        if (GetComponent<MMSimpleObjectPooler>() != null)
        {
            ObjectPooler = GetComponent<MMSimpleObjectPooler>();
        }
        if (ObjectPooler == null)
        {
            Debug.LogWarning(this.name + " : no object pooler (simple or multiple) is attached to this Projectile Weapon, it won't be able to shoot anything.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void SpawnEnemies(int amount, WaveManager waveManager)
    {
        for (int i = 0; i < amount; i++)
        {
            //int index = UnityEngine.Random.Range(0, ObjectsToInstantiate.Count);

            //spawnSingleEnemy(ObjectsToInstantiate[index], waveManager);
            Spawn();
        }
    }

    void spawnSingleEnemy(GameObject enemy, WaveManager waveManager)
    {

        _gameObject = Instantiate(enemy);
        SceneManager.MoveGameObjectToScene(_gameObject, this.gameObject.scene);
        
        //waveManager.AddEnemyToList(_gameObject);
    }


    void Spawn()
    {

        GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

        // mandatory checks
        if (nextGameObject == null) { return; }
        if (nextGameObject.GetComponent<MMPoolableObject>() == null)
        {
            throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
        }

        // we activate the object
        nextGameObject.gameObject.SetActive(true);
        nextGameObject.gameObject.MMGetComponentNoAlloc<MMPoolableObject>().TriggerOnSpawnComplete();

        // we check if our object has an Health component, and if yes, we revive our character
        Health objectHealth = nextGameObject.gameObject.MMGetComponentNoAlloc<Health>();
        if (objectHealth != null)
        {
            objectHealth.Revive();
        }

        // we position the object

        //nextGameObject.transform.position = this.transform.position;

        MoreMountains.Tools.MMSpawnAround.ApplySpawnAroundProperties(nextGameObject, SpawnProperties, this.transform.position);



    }
}

