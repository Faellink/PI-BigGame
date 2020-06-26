using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CidadaoDamage : MonoBehaviour
{

    public Image life;
    public bool damageOn;
    public float timer = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        life = GameObject.Find("Life Bar").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (damageOn)
        {
            DamageTimer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            damageOn = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            damageOn = false;
        }
    }

    void DamageTimer()
    {
        life.fillAmount -= 1.0f / timer * Time.deltaTime;
    }
}
