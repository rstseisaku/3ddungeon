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
            }
            if (img.name == "Fade2")
            {
                img.material.SetFloat("_Blend", fill);
            }
        }
    }
}
