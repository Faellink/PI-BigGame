using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController player;
    public List<Transform> spawns;
    public float randomSpawnTime;
    public List<GameObject> enemyPrefabs;
    float nextSpawn;

    private void Update()
    {
        if(Time.time > nextSpawn)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        int randomSpawn = Random.Range(0, spawns.Count);
        Vector3 spawnPos = spawns[randomSpawn].position;
        randomSpawn = Random.Range(0, enemyPrefabs.Count);
        GameObject prefab = enemyPrefabs[randomSpawn];
        var obj = Instantiate(prefab);
        obj.transform.position = spawnPos;
        obj.GetComponent<ZombieAI>().player = player;
        nextSpawn = Time.time + randomSpawnTime;

    }

}
