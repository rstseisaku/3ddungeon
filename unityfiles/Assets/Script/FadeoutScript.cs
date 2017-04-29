using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeoutScript : MonoBehaviour
{

    public float fill = 0;
    public float addNum = 0;

    Image img;

    // Use this for initialization
    void Start()
    {
        img = GetComponent<Image>();
        img.material.EnableKeyword("_Cutoff");
        img.material.EnableKeyword("_Blend");

        /* ない場合はスルーされる */
        img.material.SetFloat("_Blend", 0);
        img.material.SetFloat("_Cutoff",0);
    }

    // Update is called once per frame
    void Update()
    {
        if (fill < 1)
        {
            fill += addNum;
            if (img.name == "Fade")
            {
                img.fillAmount = fill;
                img.material.SetFloat("_Blend", fill);
            }
        }
    }
}
