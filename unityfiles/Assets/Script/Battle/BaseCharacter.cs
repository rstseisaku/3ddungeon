using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/* 属性 */
enum Element { Fire, Water, Thunder, Light, Dark }


/*
 * キャラクターの情報を管理する基底クラス
 */
public class BaseCharacter : MonoBehaviour
{
    GameObject dObj; // AddComponent を使うためのダミーオブジェクト

    // 敵・味方のフラグを立てておく
    protected bool isPlayerCharacter = false;

    // 画像関連
    public GameObject battleCanvas; // 描画対象となるキャンパス
    public string faceGraphicPath; // 顔グラのファイルパス

    public GameObject StatusObj; // ステータス画面用の顔グラオブジェクト( Image オブジェクト)

    // getter setter の定義
    public Vector3 FaceObjLocalScale
    {
        get { return ctbFaceObj.faceObj.GetComponent<RectTransform>().localScale; }
        set { ctbFaceObj.faceObj.GetComponent<RectTransform>().localScale = value; }
    }
    public Sprite FaceObjSprite
    {
        get { return ctbFaceObj.faceObj.GetComponent<Image>().sprite; }
        set { ctbFaceObj.faceObj.GetComponent<Image>().sprite = value; }
    }
    public Vector3 FaceObjLocalPosition
    {
        get { return ctbFaceObj.faceObj.GetComponent<RectTransform>().localPosition; }
        set { ctbFaceObj.faceObj.GetComponent<RectTransform>().localPosition = value; }
    }

    // キャラクターのステータスなど
    public string charaName; // キャラの名前
    public int Hp; // 体力( 雑魚戦用 )
    public int Atk; // 攻撃力
    public int Mag; // 魔力値
    public int knockback; // 吹き飛ばし力
    public int resistKnockback; // 吹き飛ばし耐性
    public bool isWaitUnison; // ユニゾン待機中か否か
    public bool isMagic; // 詠唱中か否か
    public int magWaitBase; // 詠唱待機時間のベース値
    public int magWait; // 詠唱待機時間の乱数加算後
    public int waitActionBase; // 待機時間のベース値
    public int waitAction; // 待機時間の乱数加算後
    public int element; // キャラクターの属性
    public int stunCount; // スタン時間
    public bool isknockout; // 戦闘不能フラグ
    public bool isStun; // スタンフラグ

    // CTB 関連
    public int partyId; // パーティ ID
    public int ctbNum; // CTB値
    protected int targetId; // 攻撃対象
    public int TargetId { get { return this.targetId; } }

    // 予測オブジェクト管理
    public PredictObject predictObj;

    // CTB 顔グラオブジェ
    public CtbFaceObj ctbFaceObj;


    // ダミーオブジェクトの登録
    public void Init(GameObject canvas)
    {
        dObj = GameObject.Find("Dummy");
        battleCanvas = canvas;
    }

    // キャラクターのデータ読み込み
    // FalePath: 設定フォルダの在り処
    //  ┗キャラクターデータ設定フォルダへのパス
    // characterId; キャラクターのID
    protected void LoadCharacterData(string FilePath, int characterId)
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
        Atk = 1000;
        Mag = 1; // 魔力値
        knockback = 6; // 吹き飛ばし力
        resistKnockback = UnityEngine.Random.Range(0, 5); // 吹き飛び耐性
        waitActionBase = 9; // 行動後の待機時間
        magWaitBase = 2 + 3 * partyId; // 詠唱後の待機時間
        SetWaitTime();

        // CTB 値を適当に初期化しておく
        ctbNum = (int)UnityEngine.Random.Range(3, 10);
        isMagic = false;
        isWaitUnison = false;
        isknockout = false;
        isStun = false;

        // 画像オブジェクトの定義など
        MakeCharacterGraphic();
    }

    // ウェイトに乱数補正をかける(初期化時・行動後に呼出)
    public void SetWaitTime()
    {
        magWait = magWaitBase + UnityEngine.Random.Range(-1, 1);
        waitAction = waitActionBase + UnityEngine.Random.Range(-1, 1);
    }


    /*
     * ===========================================================
     *  表示関係
     * ===========================================================
     */
    // 顔グラ(CTB)のオブジェクトをインスタンス化する
    // TODO: StatusObj は BaseCharacter で扱わない
    // TODO: もっと細かく分ける
    public void MakeCharacterGraphic()
    {
        // CTB 顔グラオブジェの追加
        ctbFaceObj = dObj.AddComponent<CtbFaceObj>();
        ctbFaceObj.Init(this); // 初期化(3オブジェクトインスタンス化)
        ctbFaceObj.SetPosY( isPlayerCharacter, partyId); // Y座標セット

        /*
         * ステータスオブジェクトの生成(CTB 顔グラオブジェ生成後)
         */
        // 予測オブジェクトの追加
        predictObj = dObj.AddComponent<PredictObject>();
        predictObj.Init(this);
        // ステータスオブジェクトの生成(CTB 顔グラオブジェ生成後)
        int statusObjY = BCV.STATUS_ENEMY_Y;
        if (isPlayerCharacter) statusObjY = BCV.STATUS_PLAYER_Y;
        MakeStatusObj(statusObjY);
    } // --- MakeCharacterGraphic()

    // HP‣顔グラフィックの表示
    public void MakeStatusObj(int Y)
    {
        // Image オブジェクト生成
        StatusObj = Utility.MyInstantiate(
            BCV.STATUSOBJ_PREFAB,
            battleCanvas);
        // 座標指定
        StatusObj.transform.localPosition = new Vector3(
            partyId * BCV.STATUS_VX + BCV.STATUS_LEFT_END, Y, 0);
        // 画像を貼る(画像のアドレス)
        GameObject img = StatusObj.transform.FindChild("FaceGra").gameObject;
        img.GetComponent<Image>().sprite =
                        ctbFaceObj.faceObj.GetComponent<Image>().sprite;
    } //---MakeStatusObj()
    public void SetStatusObj()
    {
        UpdateHp(); // HPの更新
    } //---SetStatusObj()
    private void UpdateHp()
    {
        // HP の更新
        GameObject text = StatusObj.transform.FindChild("HpText").gameObject;
        text.GetComponent<Text>().text = "" + Hp;
    } // ---UpdateHp()

    /*
     * 攻撃演出処理
     * ( この関数群を DrawBattleGraphic で利用する )
     */
    protected IEnumerator DrawBattleGraphic(BaseCharacter[] cd, ComboManager cm)
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
        GameObject dmgObj = DrawDamage( CalDamage(cm) );
        GameObject cmbObj = DrawCombo(cm);
        yield return Utility.Wait(45);

        Destroy(cmbObj);
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
        GameObject atkChara = 
            Utility.MyInstantiate(FilePath, battleCanvas, faceGraphicPath);
        // サイズ設定
        atkChara.transform.localScale =
            new Vector2(ConstantValue.BATTLE_ATTACKFACE_SIZE, ConstantValue.BATTLE_ATTACKFACE_SIZE);

        return atkChara;
    }
    private GameObject TargetGraphicDraw(BaseCharacter[] cd)
    {
        // Image オブジェクト生成
        string FilePath = "Prefabs\\Battle\\ImageBase";
        // 顔グラオブジェクトの生成
        GameObject chara = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        // 作成した Image オブジェクトにテクスチャを貼り付ける
        chara.GetComponent<Image>().sprite =
            cd[targetId].ctbFaceObj.faceObj.GetComponent<Image>().sprite;
        // Canvas を親オブジェクトに設定
        chara.transform.SetParent(battleCanvas.transform, false);
        // サイズ設定
        chara.GetComponent<Image>().transform.localScale =
            new Vector2(ConstantValue.BATTLE_ATTACKFACE_SIZE, ConstantValue.BATTLE_ATTACKFACE_SIZE);

        return chara;
    }
    protected GameObject AttackEffect(BaseCharacter[] cd)
    {
        // カーソルオブジェクトの表示
        string FilePath = "Prefabs\\Effect\\Effect" + UnityEngine.Random.Range(1,4);
        // Canvas サイズに合わせてプレハブ化してあるのでそのまま利用
        GameObject effObj = (GameObject)Instantiate(Resources.Load(FilePath));
        effObj.GetComponent<ParticleSystem>().Play();
        return effObj;
    }
    protected GameObject DrawDamage(int damage)
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
    protected GameObject DrawCombo(ComboManager cm)
    {
        // カーソルオブジェクトの表示
        string FilePath = "Prefabs\\Battle\\AttackText";
        GameObject cmbObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        cmbObj.transform.SetParent(battleCanvas.transform, false);
        cmbObj.transform.GetComponent<RectTransform>().localPosition =
            new Vector3(-400, 280, 0);
        cmbObj.GetComponent<Text>().text = cm.comboString;
        return cmbObj;
    }

    // ダメージ算出
    private int CalDamage( ComboManager cm )
    {
        int damage = Atk;
        damage = (damage * cm.magnificationDamage) / 100;
        return damage;
    }

    // 攻撃用の処理
    protected IEnumerator Attack(BaseCharacter[] cd, ComboManager cm)
    {
        // HPを削る
        cd[targetId].Hp -= CalDamage(cm);
        if (cd[targetId].Hp < 0) cd[targetId].Hp = 0;

        // 吹き飛ばし
        int blow = knockback - cd[targetId].resistKnockback;
        if (blow < 0) blow = 0;
        cd[targetId].ctbNum += blow;

        // ユニゾン・詠唱の解除
        EndUnison(cd[targetId]);
        EndMagic(cd[targetId]);

        yield return 0;
    }

    // 戦闘不能処理
    public void KnockoutEffect()
    {
        if( isknockout == false && (Hp <= 0))
        {
            // 効果音を鳴らすとか
            // はじけ飛ぶ演出とか(色々見て気持ちの良い演出を研究)

            // 戦闘不能フラグを立てる
            isknockout = true;

            // フラグを立てたので、顔グラフィックをセット
            SetFaceObjColorFromStatus(this);
        }
    }

    /* ==================================
     * ユニゾン関連の処理 
     * ================================== */
    public void StartUnison()
    {
        isWaitUnison = true;

        // 演出処理
        // FaceObj.GetComponent<Image>().material = 
        // (※ 加算描画のシェーダーを貼り付ける )

        // 演出処理
        SetFaceObjColorFromStatus(this);
    }
    public void EndUnison()
    {
        EndUnison(this);
    }
    public void EndUnison(BaseCharacter cb)
    {
        cb.isWaitUnison = false;

        // 演出処理
        SetFaceObjColorFromStatus(this);
    }

    /* ==================================
     * 詠唱関連の処理 
     * ================================== */
    public void StartMagic()
    {
        // 平均値 = 自分自身の値
        // ( 基本的に呼ばれることはないはず )
        Debug.LogError(" StartMagic() が呼ばれました ");
        StartMagic(magWait);
    }
    public void StartMagic(int aveWaitMagic)
    {
        // 詠唱開始
        isMagic = true;
        Mag += 2;
        ctbFaceObj.magText.GetComponent<Text>().text = "" + Mag;

        // ウェイト時間に乱数補正をかける
        ctbNum = aveWaitMagic;
        SetWaitTime();

        // 演出処理
        SetFaceObjColorFromStatus(this);
    }
    public void EndMagic()
    {
        EndMagic(this);
    }
    public void EndMagic(BaseCharacter cb)
    {
        if (cb.isMagic)
        {
            // 詠唱解除
            cb.isMagic = false;
            cb.Mag -= 2;
            cb.ctbFaceObj.magText.GetComponent<Text>().text = "" + Mag;

            // 演出処理
            SetFaceObjColorFromStatus(cb);
        }
    }

    /* ==================================
     * スタン関連の処理 
     * ================================== */
    public void StartStun(int stunCtbNum)
    {
        stunCount = stunCtbNum;
        isStun = true;

        // 演出処理
        SetFaceObjColorFromStatus(this);
    }
    public void EndStun()
    {
        // 演出処理
        isStun = false;
        SetFaceObjColorFromStatus(this);
    }

    // 行動終了後の処理
    // ( 各キャラクタのプレイアクションで呼ばれてる )
    protected void AfterAction()
    {
        // 個々のキャラクターでできる終了処理
        ctbNum = waitAction;
        EndUnison();
        EndMagic();

        // ウェイト時間に乱数補正をかける
        SetWaitTime();
    }


    /* ==================================================
     * CTB 顔グラオブジェへのインタフェース 
     * ================================================== */
    // 現在のステータスに応じて顔グラの色を変更
    public void SetFaceObjColorFromStatus(BaseCharacter bc) {
        ctbFaceObj.SetFaceObjColorFromStatus(bc); }
    // ctbNum に従った位置に顔グラを表示
    public void SetFaceObj() {
        ctbFaceObj.SetPosX(ctbNum); }

    /* ==================================================
     * 予測オブジェクトを表示するインタフェースを提供 
     * ================================================== */
    // 行動終了後の予測位置を表示
    public void SetPredictFromWaitAction() {
        predictObj.SetFromUntilCtbNum( this, waitAction );  }
    // 行動終了後の予測位置を表示(詠唱)
    public void SetPredictFromMagWait() {
        predictObj.SetFromUntilCtbNum(this, magWait); }
    // 行動終了後の予測位置を表示(数値から)
    public void SetPredictFromCtbNum( int ctbNum ) {
        predictObj.SetFromUntilCtbNum(this, ctbNum); }
}
