using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCidadao : MonoBehaviour
{

    Transform[] spawnPoints;
    Transform currentPoint;
    int pointIndex;

    public float spawnRate = 2.0f;
    public bool startSpawn;

    public int spawnCounter;
    [SerializeField] int spawnCounterLimit;

    GlobalSpawnController globalSpawnController;

    public GameObject cidadaoSpawn;

    // Start is called before the first frame update
    void Start()
    {
        globalSpawnController = GetComponentInParent<GlobalSpawnController>();
        spawnPoints = GetComponentsInChildren<Transform>();
        startSpawn = true;
        spawnCounter = 0;
        StartCoroutine(Spawner(spawnRate));
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnCounter == spawnCounterLimit)
        {
            startSpawn = false;
            StopCoroutine(Spawner(spawnRate));
            //Debug.Log("limite de cidadao");
        }

    }

    public IEnumerator Spawner(float spawnRate)
    {

        while (startSpawn)
        {
            pointIndex = Random.Range(1, spawnPoints.Length);
            currentPoint = spawnPoints[pointIndex];
            spawnCounter++;
            globalSpawnController.globalSpawnCounter++;
            Instantiate(cidadaoSpawn, currentPoint.position, Quaternion.identity);
            //Debug.Log(currentPoint.name);
            yield return new WaitForSeconds(spawnRate);
        }

    }
}
