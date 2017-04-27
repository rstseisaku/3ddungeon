using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * CTB の行動順を表すためのオブジェクト
 *  :構成要素
 *     顔グラフィック
 *     魔力値表示オブジェクト
 *     属性表示オブジェクト
 */
public class CtbFaceObj : MonoBehaviour
{
    // 構成要素のオブジェクト
    public GameObject faceObj;
    public GameObject magText;
    public GameObject elementObj;

    /* 初期化。オブジェクトを生成する */
    public void Init(BaseCharacter bc)
    {
        GenerateObj(bc);
    }

    /* Y座標の設定(初期化) */
    public void SetPosY( bool isPChara, int pId)
    {
        int baseY = BCV.CTB_ENEMY_UPPER;
        int vy = -1;
        if (isPChara)
        {
            baseY = BCV.CTB_PLAYER_BOTTOM;
            vy = 1;
        }
        faceObj.transform.localPosition = new Vector3(
            0, vy * pId * ConstantValue.BATTLE_FACE_VY + baseY, 0);
    }

    /* 3つのオブジェクトを生成する */
    private void GenerateObj(BaseCharacter bc)
    {
        // 顔グラオブジェクトの生成
        faceObj = Utility.MyInstantiate(
            BCV.FACE_IMAGE_PREFAB,
            bc.battleCanvas,
            bc.faceGraphicPath,
            new Vector2(BCV.FACE_SIZE, BCV.FACE_SIZE));

        // 魔力テキストの生成
        magText = Utility.MyInstantiate(
            BCV.MAG_TEXT_PREFAB,
            faceObj);
        magText.GetComponent<RectTransform>().localPosition -=
            new Vector3(-12, 12, 0);
        magText.GetComponent<Text>().text = "" + bc.Mag;

        // 属性エレメントの生成
        string iconFilePath = "Images/Icon/icon" + 0;
        elementObj = Utility.MyInstantiate(
            BCV.FACE_IMAGE_PREFAB,
            faceObj,
            iconFilePath,
            new Vector2(36, 36));
        elementObj.GetComponent<RectTransform>().localPosition -=
            new Vector3(18, -18, 0);
    }

    /* 移動させる(CTB値をもとに横座標を設定) */
    public void SetPosX(int ctbNum)
    {
        int y = (int)faceObj.transform.localPosition.y;
        faceObj.transform.localPosition = new Vector3(
            BCV.VX_PER_CTBNUM * ctbNum + BCV.CTB_LEFTEND_POS,
            y,
            0);
    }
}
