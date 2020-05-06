using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCidadao : MonoBehaviour
{

    Transform[] spawnPoints;
    Transform currentPoint;
    int pointIndex;

    public float spawnRate = 2.0f;
    bool startSpawn;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
        startSpawn = true;
        StartCoroutine(Spawner(spawnRate));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Spawner(float spawnRate)
    {

        while (startSpawn)
        {
            pointIndex = Random.Range(1, spawnPoints.Length);
            currentPoint = spawnPoints[pointIndex];
            Debug.Log(currentPoint.name);
            yield return new WaitForSeconds(spawnRate);
        }

    }
}
