using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



// ----------------------------------------
// toDo
// ----------------------------------------
// ★戻る処理
// ★コンボ
// ★ランブル
// ★リーダーの選択（ ランブル / ユニゾン など）
//　 ユニゾン処理
// ★気絶処理
//   ┗復帰予告
// 　戦闘不能処理
// 　エフェクトの表示順(よく分からない)

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
    private CharacterData[] cd; // キャラクターデータの配列
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
        dObj = new GameObject("Cube"); // AddComponent を使うためのダミーオブジェクト

        /* 味方キャラクターの読み込み */
        LoadPartyInfo(); // パーティー情報の読み込み
        LoadPlayerChara(); // パーティー情報をもとにキャラクターデータ読み込み＆初期描画

        /* 敵キャラクターの読み込み */
        LoadEnemyChara(); // 敵キャラクター情報の読み込み

        /* 初期化処理 */
        ComboInit();

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
        cd = new CharacterData[ConstantValue.playerNum]; // キャラクター DB 確保
        for (int i = 0; i < ConstantValue.playerNum; i++)
        {
            // キャラクターデータの領域確保( new は使わない )
            cd[i] = dObj.AddComponent<CharacterData>();

            // キャラクターデータをロード
            cd[i].LoadCharacterData(Party[i], i);
            
            // キャラクター画像描画初期化
            cd[i].MakeCharacterGraphic(canvas, ConstantValue.BATTLE_STATUS_PLAYERY);
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
                                                             
            // キャラクターデータをロード                                              
            enemyCd[i].LoadCharacterData(i, i);

            //  キャラクター画像描画初期化
            enemyCd[i].MakeCharacterGraphic(canvas, ConstantValue.BATTLE_STATUS_ENEMYY);
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
        while (true) {
            // 行動できるキャラが出てくるまで CTB を進行
            yield return DecideNextActionCharacter();

            // 行動可能キャラクターをカウントし、実際のアクションを行う
            yield return PlayAction();
            yield return AfterAction();
        }
    }

    // 行動できるキャラクターが出るまでループを回す
    IEnumerator DecideNextActionCharacter()
    {
        Vector2 tmp = CtbManager.CountActionableCharacter( cd, enemyCd);
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
    IEnumerator PlayAction()
    {
        // ランブル判定。( ユニゾン中のキャラは数えない )
        Vector2 countActionCharacterInfo = 
            CtbManager.CountActionableCharacter(cd, enemyCd);

        // ランブル・ユニゾン・通常アクションの 3 パターンがありえる
        if( countActionCharacterInfo.x * countActionCharacterInfo.y == 0)
        {
            // ランブルでない場合の処理
            yield return PlayActionNoRamble( countActionCharacterInfo );
        }
        else
        {
            // ランブル

            // 　1. 両サイドの和を求める
            int playerMagSum = CtbManager.GetSumMoveableMag(cd);
            int enemyMagSum = CtbManager.GetSumMoveableMag(enemyCd);

            //   2.リーダーキャラクターの選択( 1体でも発動 )
            int enemyLeader = SelectLeaderEnemy(countActionCharacterInfo);
            GameObject enemyLeaderObj = DrawEnemyLeader(enemyLeader);

            // 強制的に選択させる
            selectedLeader = -1;
            while ( selectedLeader == -1 ) yield return SelectLeaderPlayer(countActionCharacterInfo);

            DestroyObject(enemyLeaderObj); 

            int playerLeaderElement = cd[selectedLeader].element;
            int enemyLeaderElement = enemyCd[enemyLeader].element;

            //   2.2 勝敗判定
            int isPlayerWin = 1;
            if (playerMagSum < enemyMagSum) isPlayerWin = -1;
            else if (playerMagSum == enemyMagSum)
            {
                // 属性による勝敗判定を行う
                isPlayerWin = JudgeRamble(playerLeaderElement,enemyLeaderElement);
            }

            //   3. 少ない方にスタン処理＆ダメージ。
            int stunCtbNum = 7; // 仮置
            if (isPlayerWin == 1) CtbManager.CallStun(enemyCd,stunCtbNum);
            else if (isPlayerWin == -1) CtbManager.CallStun( cd, stunCtbNum);

            //   4. いい感じの派手なエフェクト。


            //   5. ユニゾン・詠唱を終了
            CtbManager.EndWaitUnison(true, cd, enemyCd);
            CtbManager.EndWaitUnison(false, cd, enemyCd);
            CtbManager.EndMagic(true, cd, enemyCd);
            CtbManager.EndMagic(false, cd, enemyCd);

            // 6. CTB 値を再設定( waitAction の値を格納 )
            CtbManager.SetCtbNum(cd, enemyCd);
        }
        yield return 0;
    }
   
    // 後処理
    IEnumerator AfterAction()
    {
        // ウェイトを挟む
        yield return Utility.Wait(30);

        // 情報を再描画
        DrawCharacterData();
    }



    /*
     * ================================================
     * PlayAction の内部処理
     * ================================================
     */
     // 通常攻撃・ユニゾンの場合に呼ばれる処理
    IEnumerator PlayActionNoRamble( Vector2 countActionCharacterInfo )
    {
        // 行動サイドのユニゾン待機を終了
        CtbManager.EndWaitUnison( countActionCharacterInfo.x >= 1, cd, enemyCd);
        // 行動人数の情報を再度取得( コマンド選択前に )
        countActionCharacterInfo =
            CtbManager.CountActionableCharacter(cd, enemyCd);

        // コマンド選択・リーダー決定・ターゲット選択を行う
        selectedCommand = -1;
        selectedLeader = -1;
        selectedTarget = -1;

        // 【要: ここの仕組みは、特にリファクタリングすべき】
        while (true)
        {
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

    // コマンド内容をもとに
    // 動けるキャラクターすべての行動(攻撃・ユニゾン・詠唱)を行う
    // 　┗ 呼出元: PlayActionNoRamble
    // 　┗ 条件: selectedCommand selectedLeader selectedTarget が決定済
    IEnumerator CallCharacterAction()
    {
        // 詠唱中キャラの平均待機値を求めておく
        int avePlayerMagWait = CtbManager.GetAverageMagWait(cd);
        int aveEnemyMagWait = CtbManager.GetAverageMagWait(enemyCd);

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
        int knockback = CtbManager.GetSumKnockback(cd);

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
        GameObject predictObj = MakePredictObj(enemyCd);

        string FilePath = "Prefabs\\Battle\\SelectTarget";
        GameObject tObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(400, 0, 0),
                            Quaternion.identity);
        tObj.GetComponent<SelectTarget>().SetParameter( enemyCd );
        tObj.transform.SetParent(canvas.transform, false);

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
                SetPredictObj(predictObj, mouseOver, enemyCd, sumKnockback); // 予測オブジェクト表示
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
        Destroy(predictObj);
    }
    /* 座標直書き DoSelectTarget  */
    /*
    public IEnumerator DoSelectTarget(int sumKnockback)
    {
        // カーソルオブジェクトを作成
        GameObject cursorObj = MakeCursorObj();
        GameObject predictObj = MakePredictObj(enemyCd);

        // 初期化
        int checkFinger = -1; // 監視指の ID
        int nowSelect = 0; // 現在選択しているターゲットID
        int newTouchSelect = 0; // 新押時の ターゲット ID
        bool finishFlag = false; // ターゲットを決定可能か否かを示す。( 新押時に選択されているターゲットが、既に選択されている場合に true となる )

        // 初期表示
        SetCursorObj(nowSelect, cursorObj);
        SetPredictObj(predictObj, nowSelect, enemyCd, sumKnockback); // 予測オブジェクト表示
        while (selectedTarget == -1)
        {
            // 新タッチがあったら
            if (mInput.existNewTouch >= 0)
            {
                // 監視指ID を登録
                checkFinger = mInput.existNewTouch; // 新推指のID

                // 入力座標Yを取得しカーソルIDを求める
                int _nowSelect = nowSelect;
                nowSelect = PosToTargetId(checkFinger);
                newTouchSelect = nowSelect;

                // 更新時のみ表示を更新
                finishFlag = true;
                if (nowSelect != _nowSelect)
                {
                    finishFlag = false; // 押す場所が変わった
                    SetCursorObj(nowSelect, cursorObj); // カーソルオブジェクト登録
                    SetPredictObj(predictObj, nowSelect, enemyCd, sumKnockback); // 予測オブジェクト表示
                }
            }
            // 新推がなければ、監視指の中身を見る
            else if (checkFinger >= 0)
            {
                // 入力座標Yを取得しカーソルIDを求める
                int _nowSelect = nowSelect;
                nowSelect = PosToTargetId(checkFinger);

                // 更新時のみ表示を更新
                if (nowSelect != _nowSelect)
                {
                    SetCursorObj(nowSelect, cursorObj); // カーソルオブジェクト登録
                    SetPredictObj(predictObj, nowSelect, enemyCd, sumKnockback); // 予測オブジェクト表示
                }

                // 離されたら
                if (mInput.existEndTouch == checkFinger)
                {
                    // 監視リストから除外
                    checkFinger = -1;

                    // 選択位置が変化していない場合
                    if (finishFlag && nowSelect == newTouchSelect)
                        selectedTarget = nowSelect;
                }
            }
            yield return 0;
        }

        Destroy(cursorObj);
        Destroy(predictObj);
        yield return 0;
    }
    */

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
    // 予測オブジェクトの操作(ターゲット選択時利用)
    private GameObject MakePredictObj(EnemyCharacterData[] enemyCd)
    {
        // 顔グラフィックオブジェクトをコピー(ダミー)
        GameObject predictObj = Instantiate(enemyCd[0].FaceObj);
        predictObj.transform.SetParent(enemyCd[0].FaceObj.transform.parent);
        predictObj.GetComponent<RectTransform>().localScale
            = enemyCd[0].FaceObj.GetComponent<RectTransform>().localScale;
        predictObj.transform.GetComponent<Image>().color =
            new Color(1.0f, 1.0f, 1.0f, 0.5f);
        return predictObj;
    }
    private void SetPredictObj(GameObject predictObj, int nowSelect, EnemyCharacterData[] enemyCd, int sumKnockback)
    {
        // 吹き飛び量を計算
        int blow = sumKnockback - enemyCd[nowSelect].resistKnockback;
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
    private int PosToTargetId(int checkFinger)
    {
        int posY = (int)mInput.touchPos[checkFinger].y;
        int nowSelect = posY - ConstantValue.BATTLE_ENEMYFACE_OFFSETY;
        nowSelect /= -1 * ConstantValue.BATTLE_FACE_VY;
        if (nowSelect < 0) nowSelect = 0;
        if (nowSelect >= ConstantValue.enemyNum) nowSelect = ConstantValue.enemyNum - 1;
        return nowSelect;
    } // --- PosToTargetId



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
        while (true)
        {
            if (selObj.GetComponent<SelectAction>().selectId >= 0)
            {
                selectedCommand = selObj.GetComponent<SelectAction>().selectId;
                break;
            }
            yield return 0;
        }
        Destroy(selObj);
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
        if (CtbManager.CountActionableMagic(cd, enemyCd) != 0)
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
            enemyCd[elId].FaceObj.GetComponent<Image>().sprite;

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
        Vector3 movePos = new Vector3(-ConstantValue.BATTLE_FACE_SIZE, 0, 0);
        movePos /= ConstantValue.BATTLE_TIME_PER_ONECTB;
        Vector3 movePosTrue;
        // 一定フレームをかけて移動演出を行う
        for (int j = 0; j < ConstantValue.BATTLE_TIME_PER_ONECTB; j++)
        {
            movePosTrue = movePos * 60 * Time.deltaTime;
            for (int i = 0; i <ConstantValue.playerNum; i++)
            {
                if( !cd[i].isWaitUnison && (cd[i].stunCount == 0) )
                    cd[i].FaceObj.transform.localPosition += movePosTrue;
            }
            for (int i = 0; i <ConstantValue.enemyNum; i++)
            {
                if ( !enemyCd[i].isWaitUnison && ( enemyCd[i].stunCount == 0))
                    enemyCd[i].FaceObj.transform.localPosition += movePosTrue;
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
        // Hpの更新
        for (int i = 0; i < ConstantValue.playerNum; i++)
        {
            cd[i].SetFaceObj(ConstantValue.BATTLE_PLAYERFACE_OFFSETY, 1);
            cd[i].SetStatusObj();
        }
        for (int i = 0; i < ConstantValue.enemyNum; i++)
        {
            enemyCd[i].SetFaceObj(ConstantValue.BATTLE_ENEMYFACE_OFFSETY, -1);
            enemyCd[i].SetStatusObj();
        }

    }

    // プレイヤーが勝利する場合に 1 敗北する場合に -1 を返す
    private int JudgeRamble( int pEle, int eEle)
    {
        int result = 1;
        if( pEle == eEle ) { result = 1; }
        return result;
    }
}
