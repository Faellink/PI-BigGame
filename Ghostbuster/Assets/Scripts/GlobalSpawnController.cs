using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSpawnController : MonoBehaviour
{
    SpawnCidadao[] spawnControladores;

    public SavingCounter savingCounter;

    GameObject[] globalCidadaoCounter;

    public int globalSpawnCounter;
    public int globalSpawnCounterLimit;

    void Awake()
    {
        savingCounter = GameObject.FindGameObjectWithTag("HQ").GetComponent<SavingCounter>();
        spawnControladores = GetComponentsInChildren<SpawnCidadao>();
        globalSpawnCounter = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        ContadorCidadaoGlobal();

        if (globalSpawnCounter == globalSpawnCounterLimit)
        {
            foreach (SpawnCidadao spawnContr in spawnControladores)
            {
                spawnContr.startSpawn = false;
                spawnContr.StopAllCoroutines();
            }
        }

        if (savingCounter.savingNumber == globalSpawnCounterLimit )
        {
            Debug.Log("salvou todo mundo");

            foreach (SpawnCidadao spawnContr in spawnControladores)
            {
                savingCounter.savingNumber = 0;
                spawnContr.startSpawn = true;
                spawnContr.spawnCounter = 0;
                spawnContr.StartCoroutine(spawnContr.Spawner(spawnContr.spawnRate));
            }

        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (SpawnCidadao spawnContr in spawnControladores)
            {
                spawnContr.startSpawn = true;
                spawnContr.spawnCounter = 0;
                spawnContr.StartCoroutine(spawnContr.Spawner(spawnContr.spawnRate));
            }
        }

    }

    void ContadorCidadaoGlobal()
    {
        globalCidadaoCounter = GameObject.FindGameObjectsWithTag("Cidadao");

        globalSpawnCounter = globalCidadaoCounter.Length;
        
    }
}
