﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderSelect : MonoBehaviour {
    // 選択されている値
    public int selectId = 0;
    public bool isDecided;
    GameObject[] buttonObj;
    CharacterData[] cd;

    // Use this for initialization
    void Start () {
    }

    public void SetParameter( CharacterData[] _cd )
    {
        // cd を設定する
        cd = _cd;

        // ボタンオブジェクトのインスタンス化
        buttonObj = new GameObject[4];

        // ボタンオブジェクト取得
        buttonObj[0] = gameObject.transform.FindChild("RightButton").gameObject;
        buttonObj[1] = gameObject.transform.FindChild("LeftButton").gameObject;
        buttonObj[2] = gameObject.transform.FindChild("CharacterGraphic").gameObject;
        buttonObj[3] = gameObject.transform.FindChild("Text").gameObject;

        // 親オブジェクトに自分自身を設定(一緒に破棄されて欲しい)
        foreach (GameObject o in buttonObj)
        {
            o.transform.SetParent(this.transform);
        }

        // 初期値の設定
        do
        {
            selectId++;
            selectId %= cd.Length;
        } while ( cd[selectId].ctbNum > 0 );

        // 顔グラ初期セット
        SetFaceGraphic();

        // テキスト設定(魔力レベルの和)
        int sum = CtbManager.GetSumMoveableMag(_cd);
        Debug.Log(buttonObj[3]);
        buttonObj[3].GetComponent<Text>().text = "詠唱 LV" + sum + "!";
    }

    public void myOnClick(int buttonId)
    {
        // 左ボタンが押された
        if( buttonId == 0)
        {
            do {
                selectId++;
                selectId %= cd.Length;
            } while ( cd[selectId].ctbNum > 0 );
        }

        // 右ボタンが押された
        if (buttonId == 1)
        {
            do
            {
                selectId += cd.Length - 1;
                selectId %= cd.Length;
            } while (cd[selectId].ctbNum > 0);
        }

        // 値が更新されたので、画像張替
        SetFaceGraphic();

        // 決定ボタンが押された
        if (buttonId == 2) isDecided = true;
        if (buttonId == 3)
        {
            selectId = -1;
            isDecided = true;
        }
    }

    private void SetFaceGraphic()
    {
        // cd が未定義なら処理を終了
        if (cd == null) return;

        // 画像張替
        buttonObj[2].GetComponent<Image>().sprite =
            cd[selectId].FaceObj.GetComponent<Image>().sprite;
    }


}