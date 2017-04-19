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
    GameObject battleCanvas; // 描画対象となるキャンパス(SetCanvasで設定)
    public string faceGraphicPath; // 顔グラファイルパス
    public Texture2D faceTexture; // 顔グラテクスチャ
    public GameObject FaceObj; // CTB 用の顔グラオブジェクト( Image オブジェクト)
    public GameObject StatusObj; // ステータス画面用の顔グラオブジェクト( Image オブジェクト)
    public GameObject magText; // 魔力値のテキスト表示

    // キャラクターのステータスなど
    public string charaName; // キャラの名前
    public int Hp; // 体力( 雑魚戦用 )
    public int Atk; // 攻撃力
    public int Mag; // 魔力値
    public int knockback; // 吹き飛ばし力
    public int resistKnockback; // 吹き飛ばし耐性
    public bool isWaitUnison; // ユニゾン待機中か否か
    public bool isMagic; // 詠唱中か否か

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
        isMagic = false;
        isWaitUnison = false;
    }

    // ファイル名からテクスチャを返す
    protected Texture2D MyGetTexture(string FilePath)
    {
        return Resources.Load<Texture2D>(FilePath);
    }


    /*
     * ===========================================================
     *  表示関係
     * ===========================================================
     */
    // 顔グラ(CTB)のオブジェクトをインスタンス化する
    public void MakeCharacterGraphic(GameObject canvas, int Y)
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

        // ステータスオブジェクトの生成
        MakeStatusObj(Y);

        // 魔力表示テキスト生成
        magText = MakeMagTextObj();
    } // --- MakeCharacterGraphic()
    // ctbNum に従った位置に顔グラフィック(CTB)を表示する
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

    // HP‣顔グラフィックの表示
    public void MakeStatusObj( int Y )
    {
        // Image オブジェクト生成
        string FilePath = "Prefabs\\Battle\\Status";
        // ステータスオブジェクトの生成
        StatusObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        // Canvas を親オブジェクトに設定
        StatusObj.transform.SetParent(battleCanvas.transform, false);
        // 座標指定
        StatusObj.transform.localPosition = new Vector3(
            partyId * ConstantValue.BATTLE_STATUS_VX + ConstantValue.BATTLE_STATUS_OFFSETX,
            Y,
            0);
        // 画像を貼る(画像のアドレス)
        GameObject img = StatusObj.transform.FindChild("FaceGra").gameObject;
        img.GetComponent<Image>().sprite =
                        FaceObj.GetComponent<Image>().sprite;
        // 表示順の設定( 値が小さいオブジェが上に表示される )
        StatusObj.transform.SetSiblingIndex(100);
    } //---MakeStatusObj()
    public void SetStatusObj( )
    {
        UpdateHp(); // HPの更新
    } //---SetStatusObj()
    private void UpdateHp()
    {
        // HP の更新
        GameObject text = StatusObj.transform.FindChild("HpText").gameObject;
        text.GetComponent<Text>().text = "" + Hp;
    } // ---UpdateHp()
    
    // 魔力表示テキストオブジェクトの生成
    // Set はCTB顔グラと同タイミング
    public GameObject MakeMagTextObj()
    {
        string FilePath = "Prefabs\\Battle\\MagText";
        GameObject magObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(12, -12, 0),
                            Quaternion.identity);
        magObj.transform.SetParent( FaceObj.transform , false);
        magObj.GetComponent<Text>().text = "" + Mag;        
        return magObj;
    }

    

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
        predictObj.GetComponent<RectTransform>().localScale
            = enemyCd[0].FaceObj.GetComponent<RectTransform>().localScale;
        predictObj.transform.GetComponent<Image>().color =
            new Color(1.0f, 1.0f, 1.0f, 0.5f);
        return predictObj;
    }
    private void SetPredictObj( GameObject predictObj, int nowSelect, EnemyCharacterData[] enemyCd)
    {
        // 吹き飛び量を計算
        int blow = knockback - enemyCd[nowSelect].resistKnockback;
        if (blow < 0) blow = 0;
        // 座標更新
        predictObj.GetComponent<RectTransform>().localPosition =
            enemyCd[nowSelect].FaceObj.GetComponent<RectTransform>().localPosition;
        predictObj.GetComponent<RectTransform>().localPosition +=
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


    /*
     * ===========================================================
     *  攻撃関係の処理
     * ===========================================================
     */
    // 攻撃用の演出
    protected IEnumerator DrawBattleGraphic(CharacterBase[] cd)
    {
        // 攻撃者の画像を貼り付ける
        GameObject atkObj = Attacker();
        yield return Utility.Wait(60);
        Destroy(atkObj);

        // 対象表示
        GameObject targetObj = TargetGraphicDraw(cd);
        yield return Utility.Wait(10);
        // 戦闘アニメーション
        GameObject effObj = AttackEffect(cd);
        yield return Utility.Wait(60);

        // ダメージ表示 
        GameObject dmgObj = DrawDamage( Atk );
        yield return Utility.Wait(45);

        Destroy(dmgObj);
        Destroy(effObj);
        Destroy(targetObj);

        yield return 0;
    }
    private GameObject Attacker()
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
        AttackChara.transform.localScale =
            new Vector2(ConstantValue.BATTLE_ATTACKFACE_SIZE, ConstantValue.BATTLE_ATTACKFACE_SIZE);

        return AttackChara;
    }
    private GameObject TargetGraphicDraw( CharacterBase[] cd)
    {
        // Image オブジェクト生成
        string FilePath = "Prefabs\\Battle\\ImageBase";
        // 顔グラオブジェクトの生成
        GameObject chara = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        // 作成した Image オブジェクトにテクスチャを貼り付ける
        chara.GetComponent<Image>().sprite =
            cd[targetId].FaceObj.GetComponent<Image>().sprite;
        // Canvas を親オブジェクトに設定
        chara.transform.SetParent(battleCanvas.transform, false);
        // サイズ設定
        chara.GetComponent<Image>().transform.localScale =
            new Vector2(ConstantValue.BATTLE_ATTACKFACE_SIZE, ConstantValue.BATTLE_ATTACKFACE_SIZE);

        return chara;
    }
    protected GameObject AttackEffect(CharacterBase[] cd)
    {
        // カーソルオブジェクトの表示
        string FilePath = "Prefabs\\Effect\\Effect3";
        // Canvas サイズに合わせてプレハブ化してあるのでそのまま利用
        GameObject effObj = (GameObject)Instantiate(Resources.Load(FilePath));
        effObj.transform.SetParent(battleCanvas.transform, false);
        effObj.GetComponent<ParticleSystem>().Play();
        return effObj;
    }

    // 攻撃用の処理
    protected IEnumerator Attack(CharacterBase[] cd)
    {
        // HPを削る
        cd[targetId].Hp -= Atk;

        // 吹き飛ばし
        int blow = knockback - cd[targetId].resistKnockback;
        if (blow < 0) blow = 0;
        cd[targetId].ctbNum += blow;

        // ユニゾン・詠唱の解除
        if (blow >= 1 && cd[targetId].isWaitUnison)
            cd[targetId].isWaitUnison = false;
        if (blow >= 1 && cd[targetId].isMagic)
            cd[targetId].isMagic = false;

        yield return 0;
    }
 
    // 行動終了後の処理
    protected void AfterAction()
    {
        // 個々のキャラクターでできること
        ctbNum = (int)UnityEngine.Random.Range(10, 12);
        isWaitUnison = false;
        isMagic = false;
    }

    // ダメージ表示
    protected GameObject DrawDamage( int damage )
    {
        // カーソルオブジェクトの表示
        string FilePath = "Prefabs\\Battle\\AttackText";
        GameObject damageObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        damageObj.transform.SetParent(battleCanvas.transform, false);
        damageObj.GetComponent<Text>().text = "" + damage;
        return damageObj;
    }
}
