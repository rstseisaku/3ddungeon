using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * 戦闘中に利用するキャラクターの情報を管理する基底クラス
 */
 // TODO: それぞれのオブジェクトに，スクリプトをアタッチ
public class BaseCharacter : MonoBehaviour
{
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
        battleCanvas = canvas;
    }

    // キャラクターのデータ読み込み
    // FalePath: 設定フォルダの在り処
    //  ┗キャラクターデータ設定フォルダへのパス
    // characterId; キャラクターのID
    protected void LoadCharacterData()
    {
        // 編成されていない場合に利用される値
        // ( null値のままになるので、nullチェックで弾く )
        ctbNum = 999999;
        ctbFaceObj = null;
        predictObj = null;
        StatusObj = null;

        /* 詠唱・ユニゾン・戦闘不能・スタンなどの初期状態 */
        isMagic = false;
        isWaitUnison = false;
        isknockout = false;

        isStun = false;
        isStun = true;
        stunCount = 2;

        /* 編成されていない場合は処理終了 */
        if (cs == null) return;
        SetWaitTime();
        ctbNum = (int)UnityEngine.Random.Range(3, 10); // CTB値初期値
        hp = cs.maxHp; // ウェイトのBase値をもとに計算      
        MakeCharacterGraphic(); // 画像オブジェクトの定義など
    }
    protected void LoadCharacterData(string FilePath, int characterId)
    {
        // csの設定をここでやる
        // ( 必要なくなったので、このままウェイト値の設定などへ飛ぶ )
        LoadCharacterData();
    }

    // ウェイトに乱数補正をかける(初期化時・行動後に呼出)
    public void SetWaitTime()
    {
        if (cs == null) return;
        magWait = cs.magWaitBase + UnityEngine.Random.Range(-1, 1);
        waitAction = cs.waitActionBase + UnityEngine.Random.Range(-1, 1);
    }


    /*
     * ===========================================================
     *  表示関係
     * ===========================================================
     */
    // 顔グラ(CTB)のオブジェクトをインスタンス化する
    public void MakeCharacterGraphic()
    {
        // CTB 顔グラオブジェの追加
        // ctbFaceObj = new CtbFaceObj();
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
        MakeStatusObj();
    } // --- MakeCharacterGraphic()

    // HP‣顔グラフィックの表示
    public void MakeStatusObj()
    {
        // 表示さきのY座標を決定
        int ShiftY = BCV.STATUS_VY * (partyId / BCV.STATUS_X_ITEM_NUM);
        int Y = BCV.STATUS_ENEMY_Y + ShiftY;
        if (isPlayerCharacter) { Y = BCV.STATUS_PLAYER_Y - ShiftY; }
        int X = ( partyId % BCV.STATUS_X_ITEM_NUM) * BCV.STATUS_VX + BCV.STATUS_LEFT_END;
        // Image オブジェクト生成
        StatusObj = Utility._Object.MyInstantiate(
            BCV.STATUSOBJ_PREFAB,
            battleCanvas);
        // 座標指定
        StatusObj.transform.localPosition = new Vector3(X, Y, 0);
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
        if (StatusObj == null) return;
        GameObject text = StatusObj.transform.FindChild("HpText").gameObject;
        text.GetComponent<Text>().text = "" + hp;
    } // ---UpdateHp()

    /*
     * 攻撃演出処理
     */
    protected IEnumerator DrawBattleGraphic(BaseCharacter[] cd, ComboManager cm)
    {
        // 攻撃者の画像を貼り付ける
        GameObject atkObj = DamageEffect.DrawAttackChara(this);
        yield return Utility._Wait.WaitFrame(30);
        Destroy(atkObj);

        // 対象表示
        DamageEffect.TargetGraphicDraw(cd[targetId]);
        yield return Utility._Wait.WaitFrame(10);
        // 戦闘アニメーション
        DamageEffect.AttackEffect(1);
        yield return Utility._Wait.WaitFrame(45);

        // ダメージ表示 
        DamageEffect.DrawDamage(DamageEffect.CalDamage(this,cm) );
        DamageEffect.DrawCombo(cm);
        yield return Utility._Wait.WaitFrame(45);

        // 消去
        DamageEffect.DestroyAllObject();

        yield return 0;
    }

    /*
     * 攻撃用の処理 
     */
    protected IEnumerator Attack(BaseCharacter[] cd, ComboManager cm)
    {
        // HPを削る
        cd[targetId].hp -= DamageEffect.CalDamage(this, cm);
        if (cd[targetId].hp < 0) cd[targetId].hp = 0;

        // ユニゾン・詠唱の解除
        EndUnison(cd[targetId]);
        EndMagic(cd[targetId]);
        EndUnison(this);
        EndMagic(this);

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

    // 行動終了後の処理
    // ( BattleMasterから呼ぶ )
    public void SetCtbNumFromWaitAction()
    {
        // CTB値を設定する
        if (ctbNum == 0 && !isWaitUnison)
        {
            ctbNum = waitAction;
            EndUnison();
            EndMagic();

            // ウェイト時間に乱数補正をかける
            SetWaitTime();
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
        stunCount = stunCtbNum; // スタンフレーム
        isStun = true; // スタンフラグをONに

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


    /* ==================================================
     * CTB 顔グラオブジェへのインタフェース 
     * ================================================== */
    // 現在のステータスに応じて顔グラの色を変更
    public void SetFaceObjColorFromStatus(BaseCharacter bc) {
        if(ctbFaceObj!=null) ctbFaceObj.SetFaceObjColorFromStatus(bc); }
    // ctbNum に従った位置に顔グラを表示
    public void SetFaceObj() {
        if(ctbFaceObj != null) ctbFaceObj.SetPosX(ctbNum); }

    /* ==================================================
     * 予測オブジェクトを表示するインタフェースを提供 
     * ================================================== */
    // 行動終了後の予測位置を表示
    public void SetPredictFromWaitAction(){
        if (predictObj != null) predictObj.SetFromNum(this, waitAction);}
    // 行動終了後の予測位置を表示(詠唱)
    public void SetPredictFromMagWait() {
        if (predictObj != null) predictObj.SetFromNum(this, magWait); }
    // 行動終了後の予測位置を表示(数値から)
    public void SetPredictFromCtbNum( int ctbNum ) {
        if (predictObj != null) predictObj.SetFromNum(this, ctbNum); }
    // X方向への移動
    public void MovePredictTowardX(float vx) {
        if (predictObj != null) predictObj.MoveTowardX(vx); }
    // 非表示に
    public void SetPredictInactive() {
        if (predictObj != null) predictObj.SetInactive();}
    // 復帰時間を予測(スタン)
    // 表示位置を「スタンカウント-1」にすると正常に動作
    // ⇒ スタン回復フレーム(=StunCount==0)の時に、表示位置(予告) は-1の位置にいてほしい
    // 　CTBでの処理順番: スタンカウントを進める ⇒ CTB計算…の順番。
    //   StunCount==1 のとき ⇒ 起点はベース位置+1の位置で良いはず
    //   何で-1するとそろうのこれ？
    //   StunCoun==0 のとき ⇒ オブジェクト本体が動き始めるはず
    //   ★なぜかStunCount=1のときに本体が動いちゃってる。
    public void SetPredictFromStun()
    {
        Debug.Log(stunCount);
        if (isStun && predictObj != null) predictObj.SetFromNum(this, stunCount);
    }
}
