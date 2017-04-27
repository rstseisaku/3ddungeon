using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEST : MonoBehaviour {

    public float fill = 0;

    Image fuck;

    // Use this for initialization
    void Start () {
        fuck = GetComponent<Image>();
        //fuck.material.shader = Shader.Find("Transition");
    }

    // Update is called once per frame
    void Update()
    {
        if (fill < 1)
        {
            fill += 0.015f;
            if (fuck.name == "Image")
            {
                fuck.material.EnableKeyword("_Cutoff");
                fuck.material.SetFloat("_Cutoff", fill);
            }
            if (fuck.name == "Image2")
            {
                fuck.fillAmount = fill;
            }
        }
    }
}
