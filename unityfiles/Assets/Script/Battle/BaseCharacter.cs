using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/* 属性 */
enum Element { Fire, Water, Thunder, Light, Dark }


/*
 * 戦闘中に利用するキャラクターの情報を管理する基底クラス
 */
public class BaseCharacter : MonoBehaviour
{
    GameObject dObj; // AddComponent を使うためのダミーオブジェクト

    // 敵・味方のフラグを立てておく
    protected bool isPlayerCharacter = false;

    // 画像関連
    public GameObject battleCanvas; // 描画対象となるキャンパス
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
    // 戦闘時以外にも利用する値で、戦闘中には書き換わらない
    public CharacterStatus cs;

    // 戦闘中利用値
    public int hp; // HP
    public bool isWaitUnison; // ユニゾン待機中か否か
    public bool isMagic; // 詠唱中か否か
    public int magWait; // 詠唱待機時間の乱数加算後
    public int waitAction; // 待機時間の乱数加算後
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
    protected void LoadCharacterData()
    {
        // ウェイトのBase値をもとに計算
        SetWaitTime();

        // CTB 値を適当に初期化しておく
        ctbNum = (int)UnityEngine.Random.Range(3, 10);
        isMagic = false;
        isWaitUnison = false;
        isknockout = false;
        isStun = false;
        hp = cs.maxHp;

        // Debug
        isStun = true;
        stunCount = 7;

        // 画像オブジェクトの定義など
        MakeCharacterGraphic();
    }
    protected void LoadCharacterData(string FilePath, int characterId)
    {
        // csの設定をここでやる
        LoadCharacterData();
    }

    // ウェイトに乱数補正をかける(初期化時・行動後に呼出)
    public void SetWaitTime()
    {
        magWait = cs.magWaitBase + UnityEngine.Random.Range(-1, 1);
        waitAction = cs.waitActionBase + UnityEngine.Random.Range(-1, 1);
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
        ctbFaceObj = gameObject.AddComponent<CtbFaceObj>();
        ctbFaceObj.Init(this); // 初期化(3オブジェクトインスタンス化)
        ctbFaceObj.SetPosY( isPlayerCharacter, partyId); // Y座標セット

        /*
         * ステータスオブジェクトの生成(CTB 顔グラオブジェ生成後)
         */
        // 予測オブジェクトの追加
        predictObj = gameObject.AddComponent<PredictObject>();
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
        text.GetComponent<Text>().text = "" + hp;
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
            Utility.MyInstantiate(FilePath, battleCanvas, cs.faceGraphicPath);
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
        int damage = cs.atk;
        damage = (damage * cm.magnificationDamage) / 100;
        return damage;
    }

    // 攻撃用の処理
    protected IEnumerator Attack(BaseCharacter[] cd, ComboManager cm)
    {
        // HPを削る
        cd[targetId].hp -= CalDamage(cm);
        if (cd[targetId].hp < 0) cd[targetId].hp = 0;

        // 吹き飛ばし
        int blow = cs.knockback - cd[targetId].cs.resistKnockback;
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
        if( isknockout == false && (hp <= 0))
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
        cs.mag += 2;
        ctbFaceObj.magText.GetComponent<Text>().text = "" + cs.mag;

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
            cb.cs.mag -= 2;
            cb.ctbFaceObj.magText.GetComponent<Text>().text = "" + cs.mag;

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
        SetPredictInactive();
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
        predictObj.SetFromNum( this, waitAction );  }
    // 行動終了後の予測位置を表示(詠唱)
    public void SetPredictFromMagWait() {
        predictObj.SetFromNum(this, magWait); }
    // 行動終了後の予測位置を表示(数値から)
    public void SetPredictFromCtbNum( int ctbNum ) {
        predictObj.SetFromNum(this, ctbNum); }
    // 復帰時間を予測(スタン)
    public void SetPredictFromStun() {
        if (isStun) predictObj.SetFromNum(this, stunCount); }
    // X方向への移動
    public void MovePredictTowardX(float vx) {
        predictObj.MoveTowardX(vx); }
    // 非表示に
    public void SetPredictInactive() {
        predictObj.SetInactive();}
}
