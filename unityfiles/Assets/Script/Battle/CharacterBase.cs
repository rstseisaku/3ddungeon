using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



enum Element { Fire, Water, Thunder, Light, Dark}


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
    public int magWaitBase; // 詠唱待機時間のベース値
    public int magWait; // 詠唱待機時間の乱数加算後
    public int waitActionBase; // 待機時間のベース値
    public int waitAction; // 待機時間の乱数加算後
    public int element; // キャラクターの属性
    public int stunCount; // スタン時間

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
        element = (int)Element.Fire;
        waitActionBase = 9; // 行動後の待機時間
        magWaitBase = 2 + 3 * partyId; // 詠唱後の待機時間
        SetWaitTime();

        // テクスチャ読込
        faceTexture = Utility.MyGetTexture(faceGraphicPath);

        // CTB 値を適当に初期化しておく
        ctbNum = (int)UnityEngine.Random.Range(0, 10);
        isMagic = false;
        isWaitUnison = false;
    }

    // ウェイトに乱数補正をかける(初期化時、行動後呼出)
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
    public void MakeCharacterGraphic(GameObject canvas, int statusObjY)
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
        SetFaceObj( ConstantValue.BATTLE_PLAYERFACE_OFFSETY , 1);

        // ステータスオブジェクトの生成
        MakeStatusObj(statusObjY);

        // 魔力表示テキスト生成
        magText = MakeMagTextObj();
    } // --- MakeCharacterGraphic()

    // ctbNum に従った位置に顔グラフィック(CTB)を表示する
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
     *  攻撃関係の処理
     * ===========================================================
     */
    // 攻撃用の演出
    protected IEnumerator DrawBattleGraphic(CharacterBase[] cd, ComboManager cm )
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
        GameObject dmgObj = DrawDamage( (Atk * cm.magnificationDamage) / 100 );
        GameObject cmbObj = DrawCombo( cm );
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

    // ユニゾン開始処理
    public void StartUnison()
    {
        isWaitUnison = true;

        // 演出処理
        // FaceObj.GetComponent<Image>().material = 
        // (※ 加算描画のシェーダーを貼り付ける )

        FaceObj.GetComponent<Image>().color =
            new Color(0.5f, 0.5f, 1.0f, 1.0f);
    }
    public void EndUnison()
    {
        EndUnison(this);
    }
    public void EndUnison( CharacterBase cb )
    {
        cb.isWaitUnison = false;

        // 演出処理
        if( !isMagic )
          cb.FaceObj.GetComponent<Image>().color
               = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    // 詠唱開始処理
    public void StartMagic()
    {
        // 平均値 = 自分自身の値
        // ( 基本的に呼ばれることはないはず )
        Debug.LogError(" StartMagic() が呼ばれました ");
        StartMagic(magWait);
    }
    public void StartMagic( int aveWaitMagic)
    {
        // 詠唱開始
        isMagic = true;
        Mag += 2;
        magText.GetComponent<Text>().text = "" + Mag;

        // ウェイト時間に乱数補正をかける
        ctbNum = aveWaitMagic;
        Debug.Log(ctbNum);
        SetWaitTime();

        // 演出処理
        FaceObj.GetComponent<Image>().color
            = new Color(1.6f, 0.6f, 0.6f, 1.0f);
    }

    // スタン処理起動
    public void StartStun(int stunCtbNum)
    {
        stunCount = stunCtbNum;

        // 演出処理
        FaceObj.GetComponent<Image>().color
            = new Color(0.3f, 0.3f, 0.3f, 1.0f);
    }

    // スタン処理終了
    public void EndStun()
    {
        // 演出処理
        FaceObj.GetComponent<Image>().color
            = new Color(1.0f, 1.9f, 1.0f, 1.0f);
    }

    public void EndMagic()
    {
        EndMagic(this);
    }

    public void EndMagic( CharacterBase cb )
    {
        if (cb.isMagic)
        {
            // 詠唱解除
            cb.isMagic = false;
            cb.Mag -= 2;
            cb.magText.GetComponent<Text>().text = "" + Mag;

            // 演出処理
            cb.FaceObj.GetComponent<Image>().color
                = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }

    // 攻撃用の処理
    protected IEnumerator Attack(CharacterBase[] cd, int md)
    {
        // HPを削る
        cd[targetId].Hp -= ( Atk * md ) / 100 ;

        // 吹き飛ばし
        int blow = knockback - cd[targetId].resistKnockback;
        if (blow < 0) blow = 0;
        cd[targetId].ctbNum += blow;

        // ユニゾン・詠唱の解除
        EndUnison( cd[targetId] );
        EndMagic( cd[targetId] );

        yield return 0;
    }
 
    // 行動終了後の処理
    protected void AfterAction()
    {
        // 個々のキャラクターでできる終了処理
        ctbNum = waitAction;
        EndUnison();
        EndMagic();

        // ウェイト時間に乱数補正をかける
        SetWaitTime();
    }

    // ダメージ表示
    protected GameObject DrawDamage( int damage)
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

    // combo表示
    protected GameObject DrawCombo( ComboManager cm )
    {
        // カーソルオブジェクトの表示
        string FilePath = "Prefabs\\Battle\\AttackText";
        GameObject cmbObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        cmbObj.transform.SetParent(battleCanvas.transform, false);
        cmbObj.transform.GetComponent<RectTransform>().localPosition =
            new Vector3( -400, 280, 0 );
        cmbObj.GetComponent<Text>().text = cm.comboString;
        return cmbObj;
    }
}
