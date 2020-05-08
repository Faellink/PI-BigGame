using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavingCounter : MonoBehaviour
{
    Text savingCounter;

    public int savingNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        savingCounter = GameObject.Find("SavingCounter").GetComponent<Text>();
        savingNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        savingCounter.text = "Pesoas Salvas: " + savingNumber;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cidadao"))
        {
            savingNumber++;
            Destroy(other.gameObject);
            Debug.Log("Salvou");
        }
    }
}
