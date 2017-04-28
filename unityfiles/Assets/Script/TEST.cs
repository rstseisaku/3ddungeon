using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEST : MonoBehaviour {

    public float time = 0.1f;

    Image transition;
    float rate;
    float temp = 0;

    // Use this for initialization
    void Start () {
        transition = GetComponent<Image>();
        rate = 1.0f / (time * 60);
        //fuck.material.shader = Shader.Find("Transition");
    }

    // Update is called once per frame
    void Update()
    {
        if (temp < 1)
        {
            temp += rate;
            if (transition.name == "Image")
            {
                transition.material.EnableKeyword("_Cutoff");
                transition.material.SetFloat("_Cutoff", temp);
            }
            if (transition.name == "Image2")
            {
                transition.fillAmount = temp;
            }
        }
    }
}
