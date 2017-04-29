using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour {

    public bool enableonplay = true;
    public float time = 1.0f;
    private float rate;
    private float fill = 0;

    public Texture2D rule;
    Image image;

    public Texture2D mask;
    [SerializeField, Range(0, 0.99f)]
    public float blackout;
    [SerializeField, Range(0, 0.99f)]
    public float whiteout;

    public enum TRANSITION_MODE
    {
        //ここに作ったものを列記
        _ALPHACUTOFF = 0,
        _BLEND = 1
    }
    public TRANSITION_MODE mode;

    // Use this for initialization
    void Start() {
        image = GetComponent<Image>();
        rate = 1.0f / (time * 60);
        switch (mode)
        {
            //モードの内容を記入
            case TRANSITION_MODE._ALPHACUTOFF:
                if (mask != null)
                    ALPHACUTOFF_MASK();
                if (mask == null)
                    ALPHACUTOFF_CUTOUT();
                break;
            case TRANSITION_MODE._BLEND:
                image.material.EnableKeyword("_Blend");
                //image.material = materials[1];
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fill < 1)
        {
            fill += rate;
            switch (mode)
            {
                case TRANSITION_MODE._ALPHACUTOFF:
                    image.material.SetFloat("_Cutoff", fill);
                    break;
                case TRANSITION_MODE._BLEND:
                    image.material.SetFloat("_Blend", fill);
                    break;
                default:
                    break;
            }
        }
        
    }

    void ALPHACUTOFF_MASK()
    {
        image.material = new Material(Shader.Find("Custom/Mask"));
        image.material.SetTexture("_Rule", rule);
        image.material.SetTexture("_Mask", mask);
    }

    void ALPHACUTOFF_CUTOUT()
    {
        image.material = new Material(Shader.Find("Custom/BWout"));
        image.material.SetTexture("_Rule", rule);
        image.material.SetFloat("_Blackout", blackout);
        image.material.SetFloat("_Whiteout", whiteout);
    }
}
