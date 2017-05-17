using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Variables;

#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif

//トランジションを行うスクリプト
//このスクリプトを張り付けるかEventスクリプトを張り付けて実行するのが望ましい
namespace TRANSITION { 
    //これ付けないといけない感じ?
    [System.Serializable]
    public class Transition : MonoBehaviour {
        //モードの指定
        public enum TRANSITION_MODE
        {
            //ここに作ったものを列記
            _MASK = 0,                  //指定した画像に変更
            _BLACKOUT = 1,              //ブラックアウト
            _WHITEOUT = 2,              //ホワイトアウト
            _COLOR_INVERSION = 3,       //色彩逆転?
            _FADEIN = 4,                //フェードイン
            _FADEOUT = 5,               //フェードアウト
            _GRAYSCALE = 6              //グレースケール変換
        }

        //インスペクタ―上で設定すべき値
        //time:時間(s)かけてトランジションを行う
        //rule:ルール画像
        //mode:トランジションを行うモード
        //mask:(MASKモードのみ)この画像に上書きする
        //blackout:ブラックアウト係数:0でそのまま、1で真っ黒
        //whiteout:ホワイトアウト係数:0でそのまま、1で真っ白

        //現状使ってない
        public bool enableonplay = true;
        //実行時間
        public float time = 1.0f; // 秒単位

        //ルール画像
        [SerializeField]
        public Texture2D rule;
        //マスク画像
        [SerializeField]
        public Texture2D mask;
        //ブラックアウト
        [SerializeField, Range(0, 0.99f)]
        public float blackout;
        //ホワイトアウト
        [SerializeField, Range(0, 0.99f)]
        public float whiteout;
        //モード
        [SerializeField]
        public TRANSITION_MODE mode;

        //変数
        private float rate;
        private float fill = 0;
        private Image image;

        // Use this for initialization
        void Start() {
            //有効にする
            Enable();
        }

    // Update is called once per frame
    void Update()
    {
        if (fill < 1)
        {
            fill += rate;

            image.material.SetFloat("_Value", fill);
        }
        if (fill >= 1)
        {
            Destroy(this);
        }
    }

    //パラメータのセット
    //Eventスクリプトで実行した時のみ実行
    public void SetParameter(Handler transition)
    {
            time = transition.time;
            rule = transition.rule;
            mask = transition.mask;
            blackout = transition.blackout;
            whiteout = transition.whiteout;
            mode = transition.mode;
    }

    public void Enable()
    {
            //対象のイメージを取得
            //現在は3Dオブジェクトに対して行う物ではない
            image = GetComponent<Image>();

            //秒あたりの加算値
            rate = 1.0f / (time * 60);

            //モードに応じた初期化処理
            switch (mode)
            {
                //モードの内容を記入
                case TRANSITION_MODE._MASK:
                    MASK();
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
                case TRANSITION_MODE._FADEOUT:
                    FADEOUT();
                    break;
                case TRANSITION_MODE._GRAYSCALE:
                    GRAYSCALE();
                    break;
                default:
                    Debug.Log("モードが設定されていません");
                    break;
            }

        }


    // マスク画像が設定されている場合
    void MASK()
    {
        image.material = new Material(Shader.Find("Custom/Mask"));
        image.material.SetTexture("_Rule", rule);
        image.material.SetTexture("_Mask", mask);
    }

    // ブラックアウト
    void BLACKOUT()
    {
        image.material = new Material(Shader.Find("Custom/BWout"));
        image.material.SetTexture("_Rule", rule);
        image.material.SetFloat("_Blackout", blackout);
    }

    // ホワイトアウト
    void WHITEOUT()
    {
        image.material = new Material(Shader.Find("Custom/BWout"));
        image.material.SetTexture("_Rule", rule);
        image.material.SetFloat("_Whiteout", whiteout);
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
    
    // フェードイン
    void FADEOUT()
    {
        image.material = new Material(Shader.Find("Custom/FadeOut"));
        image.material.SetTexture("_Rule", rule);
    }

    //グレースケール変換
    void GRAYSCALE()
    {
        image.material = new Material(Shader.Find("Custom/Grayscale"));
        image.material.SetTexture("_Rule", rule);
    }
}



/* ---- ここから拡張コード ---- */
#if UNITY_EDITOR
/**
 * Inspector拡張クラス
 */
[CustomEditor(typeof(Transition))]
public class CustomTransition : Editor
{
    public override void OnInspectorGUI()
    {
        Transition custom = target as Transition;

        custom.rule = EditorGUILayout.ObjectField("ルール画像", custom.rule, typeof(Texture2D), true) as Texture2D;
        custom.time = EditorGUILayout.FloatField("時間(s)", custom.time);
        custom.mode = (Transition.TRANSITION_MODE)EditorGUILayout.EnumPopup("種類", custom.mode);

        if (custom.mode == Transition.TRANSITION_MODE._MASK)
        {
            custom.mask = EditorGUILayout.ObjectField("マスク画像", custom.mask, typeof(Texture2D), true) as Texture2D;
        }
        if (custom.mode == Transition.TRANSITION_MODE._WHITEOUT)
        {
            custom.whiteout = EditorGUILayout.Slider("whiteout", custom.whiteout, 0, 1);
        }
        if (custom.mode == Transition.TRANSITION_MODE._BLACKOUT)
        {
            custom.blackout = EditorGUILayout.Slider("blackout", custom.blackout, 0, 1);
        }
        if (custom.mode == Transition.TRANSITION_MODE._COLOR_INVERSION)
        {

        }
        if (custom.mode == Transition.TRANSITION_MODE._FADEIN)
        {

        }
        if (custom.mode == Transition.TRANSITION_MODE._FADEOUT)
        {

        }
        if (custom.mode == Transition.TRANSITION_MODE._GRAYSCALE)
        {

        }

        EditorUtility.SetDirty(target);
    }
}
#endif
}