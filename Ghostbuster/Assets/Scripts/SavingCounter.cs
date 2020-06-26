using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavingCounter : MonoBehaviour
{
    public Text savingCounter;
    public TextMeshProUGUI savingCounterTmpro;

    GlobalSpawnController globalSpawnController;

    public int savingNumber = 0;
    public int savingNumberTotal = 0;


    // Start is called before the first frame update
    void Start()
    {
        savingCounterTmpro = GameObject.Find("Saving Counter Text (TMP)").GetComponent<TextMeshProUGUI>();
        savingCounter = GameObject.Find("SavingCounter").GetComponent<Text>();
        globalSpawnController = GameObject.FindGameObjectWithTag("GlobalSpawn").GetComponent<GlobalSpawnController>();
        savingNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        savingCounter.text = "Pesoas Salvas: " + savingNumber + "/" + globalSpawnController.globalSpawnCounterLimit + "Total: " + savingNumberTotal;
        savingCounterTmpro.text = "Pesoas Salvas:" + savingNumber + "/ " + globalSpawnController.globalSpawnCounterLimit + "Total:" + savingNumberTotal;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cidadao"))
        {
            savingNumber++;
            savingNumberTotal++;
            Destroy(other.gameObject);
            Debug.Log("Salvou");
        }
    }
}
