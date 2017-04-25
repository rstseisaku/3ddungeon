using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * 攻撃時の予測表示を管理するクラス
 */
public class PredictObject : MonoBehaviour
{
    // オブジェクト本体
    // ( このオブジェクトへのインタフェースを提供する )
    private GameObject obj;

    public Vector3 LocalScale
    {
        get { return obj.GetComponent<RectTransform>().localScale; }
        set { obj.GetComponent<RectTransform>().localScale = value; }
    }
    public Color Color
    {
        get { return obj.GetComponent<Image>().color; }
        set { obj.GetComponent<Image>().color = value; }
    }
    public Sprite Sprite
    {
        get { return obj.GetComponent<Image>().sprite; }
        set { obj.GetComponent<Image>().sprite = value; }
    }
    public Vector3 LocalPosition
    {
        get { return obj.GetComponent<RectTransform>().localPosition; }
        set { obj.GetComponent<RectTransform>().localPosition = value; }
    }


    /* 初期化。オブジェクトを生成し、非表示にする。 */
    public void Init( BaseCharacter bc )
    {
        GenerateObj(bc);
        obj.GetComponent<Transform>().gameObject.SetActive(false);
    }

    /* 行動キャラ・対象キャラをもとに吹っ飛び量を計算し、表示 */
    public void SetFromCharacterStatus(BaseCharacter targetChara, int sumKnockback)
    {
        // 吹き飛び量を計算
        int blowFrame = sumKnockback - targetChara.resistKnockback;
        if (blowFrame < 0) blowFrame = 0;
        // 求めた吹飛び量をもとに予測オブジェクトを表示
        SetFromUntilCtbNum(targetChara, blowFrame);
    }

    /* 行動できるまでのフレームを元に移動 */
    public void SetFromUntilCtbNum(BaseCharacter actionChara, int untilFrame )
    {
        // 座標を更新し、オブジェクトをアクティブに
        LocalPosition = actionChara.FaceObjLocalPosition;
        LocalPosition += new Vector3( untilFrame * BCV.VX_PER_CTBNUM, 0, 0);
        obj.SetActive(true);
    }

    /* 予測オブジェクトを全て非ｱｸﾃｨﾌﾞに */
    public static void SetInactiveAllPredictObj( BaseCharacter[] pChara,BaseCharacter[] eChara )
    {
        for (int i = 0; pChara!= null && i < pChara.Length; i++)
            pChara[i].predictObj.obj.GetComponent<Transform>().gameObject.SetActive(false);
        for (int i = 0; eChara != null && i < eChara.Length; i++)
            eChara[i].predictObj.obj.GetComponent<Transform>().gameObject.SetActive(false);
    }


    /*
     * ===============================================
     * 内部処理
     * ===============================================
     */
    /* FaceObj を生成する */
    private void GenerateObj(BaseCharacter bc)
    {
        obj = Instantiate(bc.FaceObj);
        obj.transform.SetParent(bc.FaceObj.transform.parent);
        LocalScale = bc.FaceObjLocalScale;
        Color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        Sprite = bc.FaceObjSprite;
    }
}
