using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconeCidadao : MonoBehaviour
{

    public Image cidadaoIcone;
    public Image cidadaoLife;
    public Transform target,targetLife;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CidadaoIcone();
        //CidadaoLifeIcone();
    }

    public void CidadaoIcone()
    {
        float minX = cidadaoIcone.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = cidadaoIcone.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(target.position);

        if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        cidadaoIcone.transform.position = pos;
    }

    public void CidadaoLifeIcone()
    {
        float minX = cidadaoLife.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = cidadaoLife.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(targetLife.position);

        if (Vector3.Dot((targetLife.position - transform.position), transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        cidadaoLife.transform.position = pos;
    }
}
