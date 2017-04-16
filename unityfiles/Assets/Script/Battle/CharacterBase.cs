using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * キャラクターの情報を管理するクラス
 */
public class CharacterBase : MonoBehaviour
{
    // 画像関連
    GameObject battleCanvas; // 描画対象となるキャンパス(MakeCharacterGraphicで設定)
    public GameObject FaceObj; // 顔グラオブジェクトの配列( Image オブジェクト)
    public string faceGraphicPath; // 顔グラファイルパス
    public Texture2D faceTexture; // 顔グラテクスチャ

    // キャラクターのステータスなど
    public string charaName; // キャラの名前
    public int Hp; // 体力( 雑魚戦用 )
    public int Atk; // 攻撃力
    public int Mag; // 魔力値

    // CTB 関連
    public int partyId; // パーティ ID
    public int ctbNum; // CTB値
    protected int targetId; // 攻撃対象


    // キャラクターのデータ読み込み
    // FalePath: 設定フォルダの在り処
    //  ┗キャラクターデータ設定フォルダへのパス
    // characterId; キャラクターのID
    public void LoadCharacterData( string FilePath, int characterId )
    {
        // 設定ファイルを読込
        string[] buffer;
        buffer = System.IO.File.ReadAllLines(FilePath);

        // linebuffer にキャラクターの情報( characterId 番目の )を格納
        string[] linebuffer;
        linebuffer = buffer[characterId].Split(',');

        // ステータス読込
        charaName = linebuffer[0]; // 名前
        faceGraphicPath = linebuffer[1]; // 画像パス
        Hp = int.Parse(linebuffer[2]); // 体力
        Atk = int.Parse(linebuffer[3]); // 攻撃力
        Mag = 1; // 魔力値

        // テクスチャ読込
        faceTexture = MyGetTexture(faceGraphicPath);

        // CTB 値を適当に初期化しておく
        ctbNum = (int)UnityEngine.Random.Range(0, 10);
    }

    // 顔グラのオブジェクトをインスタンス化する
    public void MakeCharacterGraphic(GameObject canvas)
    {
        // キャンバスを登録しておく
        battleCanvas = canvas;
        // Image オブジェクト生成
        string FilePath = "Prefabs\\Battle\\ImageBase";
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
    } // --- MakeCharacterGraphic()

    // ファイル名からテクスチャを返す
    protected Texture2D MyGetTexture(string FilePath)
    {
        return Resources.Load<Texture2D>(FilePath);
    }

    // ctbNum に従った位置に顔グラフィックを表示する
    public void SetFaceObj() {
        SetFaceObj(0, 1);
    }

    // ctbNum に従った位置に顔グラフィックを表示する(Y 座標をずらす機能持ち)
    public void SetFaceObj( int OffsetY, int vy )
    {
        // 0,-1,1のいずれかのパラメータに正規化
        vy /= Mathf.Abs(vy);

        // 座標の設定 Canvas(x,y)
        FaceObj.transform.localPosition =
            new Vector3(ConstantValue.BATTLE_FACE_SIZE * ctbNum - 200,
            vy * partyId * ConstantValue.BATTLE_FACE_VY + OffsetY,
            0);
    } // --- SetFaceObj( int OffsetY, int vy  )



    // 対象キャラクターの選択
    protected IEnumerator SelectTarget()
    {
        // カーソルオブジェクトの表示
        string FilePath = "Prefabs\\Battle\\ImageBase";
        GameObject cursorObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        Texture2D cursorTex = MyGetTexture("Images\\System\\cursor");
        cursorObj.GetComponent<Image>().sprite =
            Sprite.Create(cursorTex,
            new Rect(0, 0, cursorTex.width, cursorTex.height),
            Vector2.zero);
        cursorObj.transform.SetParent(battleCanvas.transform, false);
        cursorObj.GetComponent<RectTransform>().sizeDelta =
            new Vector2(1200, ConstantValue.BATTLE_FACE_SIZE);
        cursorObj.GetComponent<Image>().color
            = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        int nowSelect = 0;
        bool nowUpdate = true;
        targetId = -1;
        while ( targetId == -1 )
        {
            // ★スマホ向け・ブラウザゲー向きの入力はあとで考える
            if ( Input.GetKeyDown(KeyCode.Z))
            {

                targetId = nowSelect;
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                nowSelect += ConstantValue.enemyNum - 1;
                nowSelect %= ConstantValue.enemyNum;
                nowUpdate = true;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                nowSelect++;
                nowSelect %= ConstantValue.enemyNum;
                nowUpdate = true;
            }

            if (nowUpdate)
            {
                // 選択中の場所に表示する
                int posY = ConstantValue.BATTLE_ENEMYFACE_OFFSETY;
                posY += -1 *  nowSelect * ConstantValue.BATTLE_FACE_VY;
                cursorObj.transform.localPosition = new Vector3(
                    0,
                    posY,
                    0);
                nowUpdate = false;
            }

            yield return 0;
        }

        Destroy( cursorObj );
        yield return 0;
    }

    // 攻撃用の演出
    protected IEnumerator DrawBattleGraphic()
    {
        // Image オブジェクト生成
        string FilePath = "Prefabs\\Battle\\ImageBase";
        // 顔グラオブジェクトの生成
        GameObject AttackChara = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        // 作成した Image オブジェクトにテクスチャを貼り付ける
        AttackChara.GetComponent<Image>().sprite =
            Sprite.Create(faceTexture,
            new Rect(0, 0, faceTexture.width, faceTexture.height),
            Vector2.zero);
        // Canvas を親オブジェクトに設定
        AttackChara.transform.SetParent(battleCanvas.transform, false);
        // サイズ設定
        AttackChara.GetComponent<RectTransform>().sizeDelta =
            new Vector2(ConstantValue.BATTLE_ATTACKFACE_SIZE, ConstantValue.BATTLE_ATTACKFACE_SIZE);

        // 60 フレームウェイト
        for (int i = 0; i < 60; i++)
        {
            yield return 0;
        }
        Destroy(AttackChara);

        yield return 0;
    }
}
