using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour {

    public bool enableonplay = true;
    public float time = 1.0f; // 秒単位
    private float rate;
    public float fill = 0;

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
        _MASK = 1, // このシェーダー何してるやつ？
        _BLEND = 2,
        _BLACKOUT = 3,
        _WHITEOUT = 4,
        _COLOR_INVERSION = 5,
        _FADEIN = 6
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
            case TRANSITION_MODE._MASK:
                ALPHACUTOFF_MASK();
                break;
            case TRANSITION_MODE._BLEND:
                image.material.EnableKeyword("_Blend");
                //image.material = materials[1];
                break;
            case TRANSITION_MODE._WHITEOUT:
                WHITEOUT();
                break;
            case TRANSITION_MODE._BLACKOUT:
                BLACKOUT();
                break;
            case TRANSITION_MODE._COLOR_INVERSION:
                COLOR_INVERSION();
                break;
            case TRANSITION_MODE._FADEIN:
                FADEIN();
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
                case TRANSITION_MODE._BLEND:
                    image.material.SetFloat("_Blend", fill);
                    break;
                case TRANSITION_MODE._FADEIN:
                    image.material.SetFloat("_Blend", fill);
                    break;
                default:
                    image.material.SetFloat("_Cutoff", fill);
                    break;
            }
        }
        
    }

    // アルファカット，マスク画像が設定されている場合
    void ALPHACUTOFF_MASK()
    {
        image.material = new Material(Shader.Find("Custom/Mask"));
        image.material.SetTexture("_Rule", rule);
        image.material.SetTexture("_Mask", mask);
    }

    // アルファカット，マスク画像が設定されていない場合
    void ALPHACUTOFF_CUTOUT()
    {
        image.material = new Material(Shader.Find("Custom/BWout"));
        image.material.SetTexture("_Rule", rule);
        image.material.SetFloat("_Blackout", blackout);
        image.material.SetFloat("_Whiteout", whiteout);
    }

    // ブラックアウト
    void BLACKOUT()
    {
        image.material = new Material(Shader.Find("Custom/BWout"));
        image.material.SetTexture("_Rule", rule);
        image.material.SetFloat("_Blackout", 1.0f);
        image.material.SetFloat("_Whiteout", 0.0f);
    }

    // ホワイトアウト
    void WHITEOUT()
    {
        image.material = new Material(Shader.Find("Custom/BWout"));
        image.material.SetTexture("_Rule", rule);
        image.material.SetFloat("_Blackout", 0.0f);
        image.material.SetFloat("_Whiteout", 1.0f);
    }

    // 色彩反転
    void COLOR_INVERSION()
    {
        image.material = new Material(Shader.Find("Custom/ColorInversion"));
    }


    // フェードイン
    void FADEIN()
    {
        image.material = new Material(Shader.Find("Custom/FadeIn"));
        image.material.SetTexture("_Rule", rule);
    }
}
