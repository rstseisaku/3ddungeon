using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



// ----------------------------------------
//  : 残タスク
// ----------------------------------------
// ★戻る処理
// ★コンボ
// ★ランブル
// ★リーダーの選択（ ランブル / ユニゾン など )
// ★詠唱中の待機予測
// ★気絶処理
//   ┗(復帰予告必要？)
// ★戦闘不能処理
// ★エフェクト(CanvasとParticleとの表示順問題)
//　 ユニゾン追撃処理
// ★キャラクターの属性の定義・表示
//    ┗  ★顔グラフィック周りの表示方法変更
//        ★グラフィック・魔力レベル・属性を1つの塊として扱うべき
//    　　読み込みこれから
//　 定数・グローバル変数の管理方法
//　 リザルト画面


enum Command { Attack, Unison, Magic }

public class BattleMaster : MonoBehaviour
{
    /*
     * =========================================
     * 変数宣言
     * =========================================
     */
    // new を使わないためのダミーオブジェクト
    // (new を使うと Warnigが出るので回避する)
    GameObject dObj;

    // 戦闘中利用データ
    private GameObject canvas; // キャンパス(描画先)
    private int[] Party; // キャラクターの ID を表す配列(添字はPtId)
    private PlayerCharacter[] cd; // キャラクターデータの配列
    private EnemyCharacterData[] enemyCd; // 敵キャラクターの配列

    // コンボデータ
    ComboManager mCombo;

    // 選択された値の格納先
    private int selectedCommand; // 選択されたコマンドの値
    private int selectedLeader; // 選択されたリーダーの値
    private int selectedTarget; // 選択されたターゲットの値



    /*
     * =========================================
     * 関数宣言
     * =========================================
     */
    // 初期化に使う処理
    // ( 読み込み処理を呼び出す )
    void Start()
    {
        /* オブジェクト読み込み */
        canvas = GameObject.Find("Canvas"); // Canvas オブジェクトを取得
        dObj = GameObject.Find("Dummy"); ; // AddComponent を使うためのダミーオブジェクト

        /* 味方キャラクターの読み込み */
        LoadPartyInfo(); // パーティー情報の読み込み
        LoadPlayerChara(); // パーティー情報をもとにキャラクターデータ読み込み＆初期描画

        /* 敵キャラクターの読み込み */
        LoadEnemyChara(); // 敵キャラクター情報の読み込み

        /* 初期化処理 */
        ComboInit();
        DrawCharacterData();

        /* メインループスタート */
        StartCoroutine("MyUpdate");
    } // --- Start()

    /*
     * ================================================
     * 読み込み処理ここから
     * ================================================
     */
    // パーティー情報の設定
    private void LoadPartyInfo()
    {
        // ★本来はどっか別のクラスから読み込んでくる
        ConstantValue.playerNum = 4; // どっかから取ってくる

        // パーティー情報から、キャラクターID を読み込む。(一時的に設定)
        Party = new int[ConstantValue.playerNum]; // パーティ配列定義
        
        // キャラクターの ID を設定
        Party[0] = 8;
        Party[1] = 7;
        Party[2] = 10;
        Party[3] = 9;
    } // --- LoadPartyInfo()
    // パーティー情報をもとにキャラクターデータを読み込む
    private void LoadPlayerChara()
    {
        // 味方キャラクターの領域確保
        cd = new PlayerCharacter[ConstantValue.playerNum]; // キャラクター DB 確保
        for (int i = 0; i < cd.Length; i++)
        {
            // キャラクターデータの領域確保( new は使わない )
            cd[i] = dObj.AddComponent<PlayerCharacter>();
            cd[i].Init(canvas);

            // キャラクターデータをロード
            cd[i].LoadCharacterData(Party[i], i);
        }
    } // --- LoadPlayerChara()
    // 敵の情報を読み込む
    private void LoadEnemyChara()
    {
        ConstantValue.enemyNum = 3; // どっかから取ってくる
        enemyCd = new EnemyCharacterData[ConstantValue.enemyNum]; // キャラクター DB 確保
        for (int i = 0; i <ConstantValue.enemyNum; i++)
        {
            // キャラクターデータの領域確保( new は使わない )
            enemyCd[i] = dObj.AddComponent<EnemyCharacterData>(); // キャラクターデータの領域確保
            enemyCd[i].Init(canvas);

            // キャラクターデータをロード                                              
            enemyCd[i].LoadCharacterData(i, i);
        }
    } // --- LoadEnemyChara()
    // コンボ初期化処理
    private void ComboInit()
    {
        mCombo = new ComboManager();
        mCombo.Init();
    }


    /*
     * ================================================
     * ゲームメインループ
     * ================================================
     */
     //メインループのコルーチン
    IEnumerator MyUpdate()
    {
        /* キー入力を待つ(戦闘開始前に) */
        yield return Utility.WaitKey();

        int battleResult;
        while (true) {
            /* 行動できるキャラが出てくるまで CTB を進行 */
            yield return DecideNextActionCharacter();

            /* 行動可能キャラクターをカウントし、実際のアクションを行う*/
            yield return PlayAction();
            yield return AfterAction();

            /* 終了判定 */
            battleResult = CheckFinish();
            if (battleResult != 0) break;
        }

        /* 勝敗表示 */
        yield return Utility.Wait(60);
        if ( battleResult == 1) { yield return BattleResult.ResultWinScene(cd, canvas); }
        else { yield return BattleResult.ResultLoseScene(cd, canvas); }
        yield return BattleResult.ResultFadeout(canvas);

        /* 戦闘が終わったので元のマップに返す */
        SceneManager.LoadScene("TES");
    }

    // 行動できるキャラクターが出るまでループを回す
    IEnumerator DecideNextActionCharacter()
    {
        Vector2 tmp = OpeCharaList.CountActionableCharacter( cd, enemyCd);
        int isActionableChara = (int)tmp.x + (int)tmp.y;
        Debug.Log(tmp);
        while ( isActionableChara == 0 )
        {
            // CTBメータを 1 つ進める
            // 人数分のキャラクターデータの読込
            CtbManager.SubStun(cd, enemyCd);
            isActionableChara += CtbManager.SubCtbNum( cd );
            isActionableChara += CtbManager.SubCtbNum( enemyCd );

            // 移動演出のコルーチンを呼び出す
            yield return CtbMove();
        }
    } // --- DecideNextActionCharacter()

    // 誰かが動ける状況になると呼ばれる処理
    // ( ランブル・通常アクション・ユニゾンに振り分ける )
    IEnumerator PlayAction()
    {
        // ランブル判定。( ユニゾン中のキャラは数えない )
        Vector2 countActionCharacterInfo = 
            OpeCharaList.CountActionableCharacter(cd, enemyCd);

        // ランブル・ユニゾン・通常アクションの 3 パターンがありえる
        if( countActionCharacterInfo.x * countActionCharacterInfo.y == 0)
        {
            // ランブルでない場合の処理
            yield return PlayActionNoRamble( countActionCharacterInfo );
        }
        else
        {
            // ランブル
            yield return PlayActionRamble(countActionCharacterInfo);
        }
        yield return 0;
    }
   
    // 後処理
    IEnumerator AfterAction()
    {
        // ウェイトを挟む
        yield return Utility.Wait(30);

        // 戦闘不能判定
        OpeCharaList.KnockoutEffect(cd);
        OpeCharaList.KnockoutEffect(enemyCd);

        // 情報を再描画
        DrawCharacterData();
    }

    // 終了判定
    // 1 = プレイヤー勝利
    // -1 = 敵勝利
    // 0 = 続行
    private int CheckFinish()
    {
        if (OpeCharaList.isAllKnockout(enemyCd) ) return 1;
        if (OpeCharaList.isAllKnockout(cd)) return -1;
        return 0;
    }


    /*
     * ================================================
     * PlayAction の内部処理
     * ================================================
     */
    // 通常攻撃・ユニゾンの場合に呼ばれる処理
    // ( コマンド入力を行い、処理を実行する )
    IEnumerator PlayActionNoRamble( Vector2 countActionCharacterInfo )
    {
        // 行動サイドのユニゾン待機を終了
        OpeCharaList.EndWaitUnison( countActionCharacterInfo.x >= 1, cd, enemyCd);
        // 行動人数の情報を再度取得( コマンド選択前に )
        countActionCharacterInfo =
            OpeCharaList.CountActionableCharacter(cd, enemyCd);

        // コマンド選択・リーダー決定・ターゲット選択を行う
        selectedCommand = -1;
        selectedLeader = -1;
        selectedTarget = -1;

        // TODO: どこのコマンドを選択中かの制御をもうちょい工夫
        while (true)
        {
            // Debug.Log(selectedCommand + "," + selectedLeader + "," + selectedTarget);
            if ( selectedCommand == -1) {
                // コマンド選択を行う( 攻撃・待機・詠唱など )
                yield return SelectCommand(countActionCharacterInfo);
                continue;
            }

            if ( selectedLeader == -1)
            {
                // リーダーキャラクターの選択
                yield return SelectLeader(countActionCharacterInfo);
                if (selectedLeader == -1) selectedCommand = -1;
                continue;
            }

            if ( selectedTarget == -1) {
                // ターゲットキャラクターの選択
                yield return SelectTarget(countActionCharacterInfo);
                if (selectedTarget == -1)
                {
                    selectedLeader = -1;
                    if (countActionCharacterInfo.x == 1 ||
                        countActionCharacterInfo.y == 1 ||
                        selectedCommand == (int)Command.Magic)
                    {
                        selectedCommand = 0; // リーダー選択が存在しないならば 2 ステップ戻す
                        yield break;
                    }
                }
                continue;
            }

            break;
        }

        // 入力内容に応じて、処理を実行する
        yield return CallCharacterAction();
    }

    IEnumerator PlayActionRamble( Vector2 countActionCharacterInfo)
    {
        // 敵のリーダーキャラクターの選択( 1体でも発動 )
        int enemyLeader = SelectLeaderEnemy(countActionCharacterInfo);
        GameObject enemyLeaderObj = DrawEnemyLeader(enemyLeader);

        // 味方のリーダーキャラクター選択
        selectedLeader = -1;
        while ( selectedLeader == -1) yield return SelectLeaderPlayer(countActionCharacterInfo);
        DestroyObject(enemyLeaderObj);

        // 勝敗判定
        int isPlayerWin = JudgeRamble(enemyCd[enemyLeader].element);

        // 少ない方にスタン処理＆ダメージ。
        int stunCtbNum = 7; // 仮置
        if (isPlayerWin == 1) OpeCharaList.CallStun(enemyCd, stunCtbNum);
        else if (isPlayerWin == -1) OpeCharaList.CallStun(cd, stunCtbNum);

        //  エフェクト表示

        //  ユニゾン・詠唱を終了
        OpeCharaList.EndWaitUnison(true, cd, enemyCd);
        OpeCharaList.EndWaitUnison(false, cd, enemyCd);
        OpeCharaList.EndMagic(true, cd, enemyCd);
        OpeCharaList.EndMagic(false, cd, enemyCd);

        // CTB 値を再設定( waitAction の値を格納 )
        CtbManager.SetCtbNum(cd, enemyCd);
    }

    // コマンド内容をもとに
    // 動けるキャラクターすべての行動(攻撃・ユニゾン・詠唱)を行う
    // 　┗ 呼出元: PlayActionNoRamble
    // 　┗ 条件: selectedCommand selectedLeader selectedTarget が決定済
    IEnumerator CallCharacterAction()
    {
        // 詠唱中キャラの平均待機値を求めておく
        int avePlayerMagWait = OpeCharaList.GetAverageMagWait(cd);
        int aveEnemyMagWait = OpeCharaList.GetAverageMagWait(enemyCd);

        for (int i = 0; i < cd.Length; i++)
        {
            // 味方キャラクター。CTB ゲージが 0 の場合。
            if (cd[i].ctbNum <= 0 && !cd[i].isWaitUnison)
            {
                // コマンドに応じた行動を行う
                if (selectedCommand == (int)Command.Attack)
                {

                    mCombo.AddPlayerCombo();
                    yield return cd[i].PlayAction(
                        selectedTarget,
                        enemyCd,
                        mCombo);
                }
                else if (selectedCommand == (int)Command.Unison)
                {
                    cd[i].StartUnison();
                }
                else if (selectedCommand == (int)Command.Magic)
                {
                    cd[i].StartMagic(avePlayerMagWait);
                }
            }
        }
        for (int i = 0; i < enemyCd.Length; i++)
        {
            // 敵キャラクター。CTB ゲージが 0 の場合。
            if (enemyCd[i].ctbNum <= 0 && !enemyCd[i].isWaitUnison)
            {
                if (selectedCommand == (int)Command.Attack)
                {
                    mCombo.AddEnemyCombo();
                    yield return enemyCd[i].PlayAction(
                        selectedTarget,
                        cd,
                        mCombo);
                }
                else if (selectedCommand == (int)Command.Unison)
                {
                    enemyCd[i].StartUnison();
                }
                else if (selectedCommand == (int)Command.Magic)
                {
                    enemyCd[i].StartMagic(aveEnemyMagWait);
                }
            }
        }
    }



    /*
     * ===========================================================
     *  対象キャラクターの選択
     * ===========================================================
     */
    // ターゲットの選択を行う
    // 　┗ 呼出元: PlayActionNoRamble
    // 　┗ 条件: selectedCommand selectedLeader が決定済
    IEnumerator SelectTarget(Vector2 countActionCharacterInfo)
    {
        // 味方キャラが行動できないなら処理終了( 敵キャラの値を設定 )
        if (countActionCharacterInfo.x == 0)
        {
            selectedTarget = UnityEngine.Random.Range(0, cd.Length - 1);
            yield break;
        }

        // ノックバック値の和を求める
        int knockback = OpeCharaList.GetSumKnockback(cd);

        // 攻撃コマンドの場合のみターゲット選択を行う
        if (selectedCommand == (int)Command.Attack)
        {
            yield return DoSelectTarget2(knockback);
        }
        else
        {
            selectedTarget = 0; // 攻撃コマンドでない場合、決定済として扱う
        }
    }
    /* ボタンクリック式 */
    IEnumerator DoSelectTarget2(int sumKnockback)
    {
        // カーソルオブジェクトを作成
        GameObject cursorObj = MakeCursorObj();
        // GameObject predictObj = MakePredictObj(enemyCd);

        string FilePath = "Prefabs\\Battle\\SelectTarget";
        GameObject tObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(400, 0, 0),
                            Quaternion.identity);
        tObj.GetComponent<SelectTarget>().SetParameter( enemyCd );
        tObj.transform.SetParent(canvas.transform, false);

        // プレイヤの予測オブジェクトを表示
        OpeCharaList.SetPredictActionableCharacter(cd);

        // コマンドを選ぶまでループ
        int _mouseOver = -1;
        int mouseOver = 0;
        while (true)
        {
            // 予測表示
            mouseOver = tObj.GetComponent<SelectTarget>().mouseOverId;
            if( _mouseOver != mouseOver)
            {
                SetCursorObj(mouseOver, cursorObj); // カーソルオブジェクト登録

                PredictObject.SetInactiveAllPredictObj( null , enemyCd);
                enemyCd[mouseOver].predictObj.SetFromCharacterStatus(enemyCd[mouseOver], sumKnockback);
                _mouseOver = mouseOver;
            }

            // 終了判定
            if (tObj.GetComponent<SelectTarget>().selectId != -1)
            {
                selectedTarget = tObj.GetComponent<SelectTarget>().selectId;
                if ( selectedTarget < 0) selectedTarget = -1; // キャンセルした
                break;
            }
            yield return 0;
        }
        Destroy(tObj);
        Destroy(cursorObj);

        PredictObject.SetInactiveAllPredictObj(cd,enemyCd);
    }

    // カーソルオブジェクトの操作(ターゲット選択時利用)
    private GameObject MakeCursorObj()
    {
        // カーソルオブジェクトの表示
        string FilePath = "Prefabs\\Battle\\ImageBase";
        GameObject cursorObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        Texture2D cursorTex = Utility.MyGetTexture("Images\\System\\cursor");
        cursorObj.GetComponent<Image>().sprite =
            Sprite.Create(cursorTex,
            new Rect(0, 0, cursorTex.width, cursorTex.height),
            Vector2.zero);
        cursorObj.transform.SetParent(canvas.transform, false);
        cursorObj.GetComponent<RectTransform>().sizeDelta =
            new Vector2(1200, ConstantValue.BATTLE_FACE_SIZE);
        cursorObj.GetComponent<Image>().color
            = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        return cursorObj;
    } // ---MakeCursorObj
    private void SetCursorObj(int nowSelect, GameObject cursorObj)
    {
        int posY = ConstantValue.BATTLE_ENEMYFACE_OFFSETY;
        posY += -1 * nowSelect * ConstantValue.BATTLE_FACE_VY;
        cursorObj.transform.localPosition = new Vector3(
            0,
            posY,
            0);
    } // ---SetCursorObj
    


    /*
     * ================================================
     * コマンド選択 / リーダーキャラクター選択
     * ================================================
     */
    // 行動内容選択(アタック・ユニゾン・攻撃)
    // 　設定方法: vec( int, int, int ) で選択肢を
    //             表示するか否かを示す
    private IEnumerator DoSelectCommand( Vector3 vec)
    {
        string FilePath = "Prefabs\\Battle\\SelectAction";
        GameObject selObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(400, 0, 0),
                            Quaternion.identity);
        selObj.GetComponent<SelectAction>().SetParameter(vec);
        selObj.transform.SetParent(canvas.transform, false);

        // コマンドを選ぶまでループ
        int mouseOverId = 0;
        int _mouseOverId = -1;
        while (true)
        {
            // マウスオーバーに基づく予測表示処理
            if ( mouseOverId != _mouseOverId)
            {
                PredictObject.SetInactiveAllPredictObj(cd,null);
                if (mouseOverId == 0) OpeCharaList.SetPredictActionableCharacter(cd);
                if (mouseOverId == 2) OpeCharaList.SetMagPredictActionableCharacter(cd);
            }
            mouseOverId = selObj.GetComponent<SelectAction>().mouseOverId;

            // 決定処理
            if ( selObj.GetComponent<SelectAction>().selectId >= 0)
            {
                selectedCommand = selObj.GetComponent<SelectAction>().selectId;
                break;
            }
            yield return 0;
        }
        Destroy(selObj);
        PredictObject.SetInactiveAllPredictObj(cd,null);
    }
    private IEnumerator SelectCommand( Vector2 countActionCharacterInfo)
    {
        // 味方キャラが行動できないなら処理終了
        if (countActionCharacterInfo.x == 0)
        {
            // 敵キャラクターの行動は通常攻撃のみ(仮)
            selectedCommand = 0;
            yield break;
        }

        // 基本的に全項目選択可能
        int isMagicEnable = 1;
        int isUnisonEnable = 1;
        // ユニゾン状態の場合、ユニゾンを選択不可に
        if (countActionCharacterInfo.x >= 2) isUnisonEnable = 0;
        // 詠唱中キャラが含まれていたら 攻撃 のみ選択可
        if (OpeCharaList.CountActionableMagic(cd, enemyCd) != 0)
        {
            isMagicEnable = 0;
            isUnisonEnable = 0;
        }
        // 選択肢表示処理実行
        yield return DoSelectCommand( new Vector3(1, isUnisonEnable, isMagicEnable));
    }

    // リーダーキャラクターの選択
    // 条件: selectedCommand と countActionCharacterInfo が決定済
    private IEnumerator DoSelectLeader()
    {
        // リーダーキャラクター選択処理
        string FilePath = "Prefabs\\Battle\\LeaderSelect";
        GameObject selObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(400, 0, 0),
                            Quaternion.identity);
        selObj.GetComponent<LeaderSelect>().SetParameter(cd);
        selObj.transform.SetParent(canvas.transform, false);

        // コマンドを選ぶまでループ
        while (true)
        {
            if (selObj.GetComponent<LeaderSelect>().isDecided)
            {
                selectedLeader = selObj.GetComponent<LeaderSelect>().selectId;
                break;
            }
            yield return 0;
        }
        Destroy(selObj);
    }
    private IEnumerator SelectLeader(Vector2 countActionCharacterInfo)
    {
        // ユニゾンでない場合・詠唱の場合は、処理を終了        
        if (countActionCharacterInfo.x == 1 ||
            countActionCharacterInfo.y == 1 ||
            selectedCommand == (int)Command.Magic )
        {
            selectedLeader = 0; // リーダー決定済として扱う
            yield break;
        }

        // 味方キャラが行動できないなら処理終了( 敵キャラの値を設定 )
        if (countActionCharacterInfo.x == 0)
        {
            // 敵キャラクターのリーダーは番号が若いキャラ
            for (int i = 0; i < enemyCd.Length; i++)
            {
                if (enemyCd[i].ctbNum == 0) selectedLeader = i;
            }
            yield break;
        }
        // リーダー選択処理の本体を呼ぶ
        yield return DoSelectLeader();
    }
    private IEnumerator SelectLeaderPlayer(Vector2 countActionCharacterInfo)
    {
        // 味方キャラクターのリーダーを決定。
        // 行動可能キャラが 1 体であってもかならず起動する。

        // 味方キャラが行動できないなら処理終了( 敵キャラの値を設定 )
        if (countActionCharacterInfo.x == 0)
        {
            // 敵キャラクターのリーダーは番号が若いキャラ
            for (int i = 0; i < enemyCd.Length; i++)
            {
                if (enemyCd[i].ctbNum == 0) selectedLeader = i;
            }
            yield break;
        }

        // リーダー選択処理の本体を呼ぶ
        yield return DoSelectLeader();
    }
    private int SelectLeaderEnemy(Vector2 countActionCharacterInfo)
    {
        int selectedId = -1;
        if (countActionCharacterInfo.y >= 1)
        {
            // 敵キャラクターのリーダーは番号が若いキャラ
            for (int i = 0; i < enemyCd.Length; i++)
            {
                if (enemyCd[i].ctbNum == 0)
                {
                    selectedId = i;
                    break;
                }
            }
        }
        return selectedId;
    }
    private GameObject DrawEnemyLeader( int elId)
    {
        // リーダーキャラクター選択処理
        string FilePath = "Prefabs\\Battle\\EnemyLeader";
        GameObject eObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(-400, 0, 0),
                            Quaternion.identity);
        eObj.transform.SetParent(canvas.transform, false);

        GameObject ecObj = eObj.transform.FindChild("CharacterGraphic").gameObject;
        ecObj.GetComponent<Image>().sprite = 
            enemyCd[elId].ctbFaceObj.faceObj.GetComponent<Image>().sprite;

        ecObj = eObj.transform.FindChild("Text").gameObject;
        ecObj.GetComponent<Text>().text =
            "詠唱 LV" + OpeCharaList.GetSumMoveableMag(enemyCd) + "!";

        return eObj;
    }

    /*
     * ================================================
     *  その他
     * ================================================
     */
    // CTB の進行演出(キャラクターグラフィックを左に動かす)
    IEnumerator CtbMove()
    {
        // 1フレームあたりに動かす移動量を算出
        Vector3 movePos = new Vector3(-BCV.VX_PER_CTBNUM , 0, 0);
        movePos /= BCV.FRAME_PER_CTB;
        Vector3 movePosTrue;
        // 一定フレームをかけて移動演出を行う
        for (int j = 0; j < BCV.FRAME_PER_CTB; j++)
        {
            movePosTrue = movePos * 60 * Time.deltaTime;
            for (int i = 0; i <cd.Length; i++)
            {
                if( !cd[i].isWaitUnison && (cd[i].stunCount == 0) && (cd[i].Hp != 0) )
                    cd[i].ctbFaceObj.faceObj.transform.localPosition += movePosTrue;
            }
            for (int i = 0; i <enemyCd.Length; i++)
            {
                if ( !enemyCd[i].isWaitUnison && ( enemyCd[i].stunCount == 0) && (enemyCd[i].Hp != 0))
                    enemyCd[i].ctbFaceObj.faceObj.transform.localPosition += movePosTrue;
            }
            yield return 0;
        }
        // 移動後に再描画
        DrawCharacterData();
        yield return 0;
    } // --- CtbMove()

    // キャラクター情報の描画を更新
    private void DrawCharacterData()
    {
        // CTB に応じた位置に再描画( CTB 顔グラ )
        // HPの更新
        for (int i = 0; i < ConstantValue.playerNum; i++)
        {
            cd[i].SetFaceObj();
            cd[i].SetStatusObj();
        }
        for (int i = 0; i < ConstantValue.enemyNum; i++)
        {
            enemyCd[i].SetFaceObj();
            enemyCd[i].SetStatusObj();
        }

    }

    // プレイヤーが勝利する場合に 1 敗北する場合に -1 を返す
    private int JudgeRamble(int eEle)
    {
        int isPlayerWin = 1;

        // それぞれのサイドのMag値の和を求める
        int playerMagSum = OpeCharaList.GetSumMoveableMag(cd);
        int enemyMagSum = OpeCharaList.GetSumMoveableMag(enemyCd);
        int pEle = cd[selectedLeader].element;

        // 魔力レベル合計値のよる勝敗判定
        if (playerMagSum < enemyMagSum)
        {
            isPlayerWin = -1;
            return isPlayerWin;
        }

        // 属性による勝敗判定
        if( pEle == eEle ) { isPlayerWin = 1; }
        return isPlayerWin;
    }
}
