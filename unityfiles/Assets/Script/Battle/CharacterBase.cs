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
    public int knockback; // 吹き飛ばし力
    public int resistKnockback; // 吹き飛ばし耐性

    // CTB 関連
    public int partyId; // パーティ ID
    public int ctbNum; // CTB値
    protected int targetId; // 攻撃対象
    public int TargetId { get { return this.targetId; }}


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
        knockback = 6; // 吹き飛ばし力
        resistKnockback = UnityEngine.Random.Range(0, 5); // 吹き飛び耐性

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
    public void SetFaceObj( int OffsetY, int vy )
    {
        // 0,-1,1のいずれかのパラメータに正規化
        vy /= Mathf.Abs(vy);

        // 座標の設定 Canvas(x,y)
        FaceObj.transform.localPosition =
            new Vector3(ConstantValue.BATTLE_FACE_SIZE * ctbNum + ConstantValue.BATTLE_FACE_OFFSETX,
            vy * partyId * ConstantValue.BATTLE_FACE_VY + OffsetY,
            0);
    } // --- SetFaceObj( int OffsetY, int vy  )



    /*
     * ===========================================================
     *  対象キャラクターの選択(targetId を戻り値として利用)
     * ===========================================================
     */
    // 対象キャラクターの選択処理実部
    protected IEnumerator SelectTarget( EnemyCharacterData[] enemyCd )
    {
        // カーソルオブジェクトを作成
        GameObject cursorObj = MakeCursorObj();
        GameObject predictObj = MakePredictObj( enemyCd );

        // 初期化
        int checkFinger = -1; // 監視指の ID
        int nowSelect = 0; // 現在選択しているターゲットID
        int newTouchSelect = 0; // 新押時の ターゲット ID
        bool finishFlag = false; // ターゲットを決定可能か否かを示す。( 新押時に選択されているターゲットが、既に選択されている場合に true となる )

        // 初期表示
        SetCursorObj(nowSelect, cursorObj);
        SetPredictObj(predictObj, nowSelect, enemyCd); // 予測オブジェクト表示
        while ( targetId == -1 )
        {
            // 新タッチがあったら
            if ( mInput.existNewTouch >= 0 )
            {
                // 監視指ID を登録
                checkFinger = mInput.existNewTouch; // 新推指のID

                // 入力座標Yを取得しカーソルIDを求める
                int _nowSelect = nowSelect;
                nowSelect = PosToTargetId( checkFinger );
                newTouchSelect = nowSelect;

                // 更新時のみ表示を更新
                finishFlag = true;
                if (nowSelect != _nowSelect)
                {
                    finishFlag = false; // 押す場所が変わった
                    SetCursorObj(nowSelect, cursorObj); // カーソルオブジェクト登録
                    SetPredictObj(predictObj, nowSelect, enemyCd); // 予測オブジェクト表示
                }
            }
            // 新推がなければ、監視指の中身を見る
            else if( checkFinger >= 0)
            {
                // 入力座標Yを取得しカーソルIDを求める
                int _nowSelect = nowSelect;
                nowSelect = PosToTargetId( checkFinger );

                // 更新時のみ表示を更新
                if (nowSelect != _nowSelect)
                {
                    SetCursorObj(nowSelect, cursorObj); // カーソルオブジェクト登録
                    SetPredictObj(predictObj, nowSelect, enemyCd); // 予測オブジェクト表示
                }

                // 離されたら
                if (mInput.existEndTouch == checkFinger)
                {
                    // 監視リストから除外
                    checkFinger = -1;

                    // 選択位置が変化していない場合
                    if ( finishFlag &&  nowSelect == newTouchSelect)
                        targetId = nowSelect;
                }
            }
            yield return 0;
        }
        Destroy( cursorObj );
        Destroy( predictObj );
        yield return 0;
    }
    // カーソルオブジェクトの操作(ターゲット選択時利用)
    private GameObject MakeCursorObj()
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
        return cursorObj;
    } // ---MakeCursorObj
    private void SetCursorObj( int nowSelect, GameObject cursorObj )
    {
        int posY = ConstantValue.BATTLE_ENEMYFACE_OFFSETY;
        posY += -1 * nowSelect * ConstantValue.BATTLE_FACE_VY;
        cursorObj.transform.localPosition = new Vector3(
            0,
            posY,
            0);
    } // ---SetCursorObj
    // 予測オブジェクトの操作(ターゲット選択時利用)
    private GameObject MakePredictObj( EnemyCharacterData[] enemyCd)
    {
        // 顔グラフィックオブジェクトをコピー(ダミー)
        GameObject predictObj = GameObject.Instantiate(
            enemyCd[0].FaceObj);
        predictObj.transform.SetParent(enemyCd[0].FaceObj.transform.parent);
        predictObj.transform.localScale = enemyCd[0].FaceObj.transform.localScale;
        predictObj.transform.GetComponent<Image>().color =
            new Color(1.0f, 1.0f, 1.0f, 0.5f);
        return predictObj;
    }
    private void SetPredictObj( GameObject predictObj, int nowSelect, EnemyCharacterData[] enemyCd)
    {
        Debug.Log(nowSelect);
        // 吹き飛び量を計算
        int blow = knockback - enemyCd[nowSelect].resistKnockback;
        if (blow < 0) blow = 0;
        // 座標更新
        predictObj.transform.position = enemyCd[nowSelect].FaceObj.transform.position;
        predictObj.transform.position +=
            new Vector3(blow * ConstantValue.BATTLE_FACE_SIZE, 0, 0);
        // Sprite 貼り付け
        predictObj.GetComponent<Image>().sprite =
            enemyCd[nowSelect].FaceObj.GetComponent<Image>().sprite;

    }
    // ターゲット ID を算出
    private int PosToTargetId( int checkFinger )
    {
        int posY = (int)mInput.touchPos[checkFinger].y;
        int nowSelect = posY - ConstantValue.BATTLE_ENEMYFACE_OFFSETY;
        nowSelect /= -1 * ConstantValue.BATTLE_FACE_VY;
        if (nowSelect < 0) nowSelect = 0;
        if (nowSelect >= ConstantValue.enemyNum) nowSelect = ConstantValue.enemyNum - 1;
        return nowSelect;
    } // --- PosToTargetId



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

    // 攻撃実処理
    protected void Attack( EnemyCharacterData[] enemyCd )
    {
        // HPを削る
        enemyCd[targetId].Hp -= Atk;

        // 吹き飛ばし
        int blow = knockback - enemyCd[targetId].resistKnockback;
        if (blow < 0) blow = 0;
        enemyCd[targetId].ctbNum += blow;
    }
}
