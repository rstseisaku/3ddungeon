using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterData : MonoBehaviour {
    // キャラクターの情報を色々と管理するクラス
    public GameObject FaceObj; // 顔グラオブジェクトの配列(添字はPtId)
    public Texture2D faceTexture; // 顔グラテクスチャの配列(添字はPtId)
    public string charaName; // キャラの名前
    public int partyId; // パーティの ID
    public int ctbNum; // CTB値

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void LoadCharacterData(int characterId, int pId)
    {
        // 顔のテクスチャ読込
        faceTexture = GetCharacterTexture(characterId);
        // パーティーの何番目にいるのかを設定
        partyId = pId;
        // CTB 値を適当に初期化しておく
        ctbNum = (int)UnityEngine.Random.Range(0, 10);
    }

    // 顔グラのオブジェクトをインスタンス化する
    public void MakeCharacterGraphic( GameObject canvas )
    {
        // 人数分の Image オブジェクト生成
        string FilePath = "Prefabs\\Battle\\FaceImage";
        // 顔グラオブジェクトの生成
        FaceObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        // 作成した Image オブジェクトにテクスチャを貼り付ける
        FaceObj.GetComponent<Image>().sprite =
            Sprite.Create(faceTexture,
            new Rect(0, 0, faceTexture.width, faceTexture.height),
            Vector2.zero);
        // Canvas を親オブジェクトに設定
        FaceObj.transform.SetParent(canvas.transform, false);
        // サイズ設定
        FaceObj.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(ConstantValue.BATTLE_FACE_SIZE, ConstantValue.BATTLE_FACE_SIZE);
        // 顔グラのをCTBに応じた位置に表示
        SetFaceObj();
    }

    // キャラクター ID から顔グラフィックのテクスチャを受け取る
    private Texture2D GetCharacterTexture(int characterId)
    {
        Texture2D tex;
        string FilePath = "Assets\\Resources\\CharacterData\\data.csv";

        string[] buffer;
        string[] linebuffer;
        buffer = System.IO.File.ReadAllLines(FilePath);

        // 顔グラの共通パスが 0行目の0セルに
        linebuffer = buffer[0].Split(',');
        string basePath = linebuffer[0];

        // 顔グラのファイル名は 3 行目の ID セルに
        linebuffer = buffer[3].Split(',');

        tex = Resources.Load<Texture2D>( basePath + linebuffer[characterId]);
        return tex;
    }

    // ctbNum に従った位置に顔グラフィックを表示する
    public void SetFaceObj()
    {
        // 座標の設定 Canvas(x,y)
        FaceObj.transform.localPosition =
            new Vector3(ConstantValue.BATTLE_FACE_SIZE * ctbNum - 200,
            partyId * ConstantValue.BATTLE_FACE_SIZE,
            0);
    }
}
