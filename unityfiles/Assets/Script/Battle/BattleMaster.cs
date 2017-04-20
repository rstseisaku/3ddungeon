using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



// ----------------------------------------
// toDo
// ----------------------------------------
// 　ランブル
// ★リーダーの選択（ ランブル / ユニゾン など）
//　 ユニゾン処理
// 　気絶処理
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
    // (new を使うと Warnigが出るので回避)
    GameObject dObj;

    // 戦闘中利用データ
    private GameObject canvas; // キャンパス(描画先)
    private int[] Party; // キャラクターの ID を表す配列(添字はPtId)
    private CharacterData[] cd; // キャラクターデータの配列
    private EnemyCharacterData[] enemyCd; // 敵キャラクターの配列

    // コンボ情報
    private int playerCombo; // プレイヤーのコンボ数
    private int enemyCombo; // 敵のコンボ数

    // 選択された値の格納先
    private int selectedCommand; // 選択されたコマンドの値
    private int selectedLeader; // 選択されたリーダーの値



    /*
     * =========================================
     * 関数宣言
     * =========================================
     */
    // 初期化に使う処理
    // 読み込み処理を呼び出す
    void Start()
    {
        // 初期化処理
        canvas = GameObject.Find("Canvas"); // Canvas オブジェクトを取得
        dObj = new GameObject("Cube"); // AddComponent を使うためのダミーオブジェクト

        // 味方キャラクターの読み込み
        LoadPartyInfo(); // パーティー情報の読み込み
        LoadPlayerChara(); // パーティー情報をもとにキャラクターデータ読み込み＆初期描画

        // 敵キャラクターの読み込み
        LoadEnemyChara(); // 敵キャラクター情報の読み込み

        // 初期化処理
        Init();

        // メインループスタート
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
    // その他の初期化処理
    private void Init()
    {
        enemyCombo = 0;
        playerCombo = 0;
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
        Vector2 tmp = CtbManager.CountActionableCharacter(true, cd, enemyCd);
        int isActionableChara = (int)tmp.x + (int)tmp.y;
        Debug.Log(tmp);
        while ( isActionableChara == 0 )
        {
            // CTBメータを 1 つ進める
            // 人数分のキャラクターデータの読込
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
            CtbManager.CountActionableCharacter(true, cd, enemyCd);

        // ランブル・ユニゾン・通常アクションの 3 パターンがありえる
        if( countActionCharacterInfo.x * countActionCharacterInfo.y == 0)
        {
            // ランブルでない場合の処理
            yield return PlayActionNoRamble( countActionCharacterInfo );
        }
        else
        {
            // ランブル

            // 　1. それぞれの魔力レベルを取得
            //   2. 両サイドの和を求める
            //   3. 少ない方にスタン処理＆ダメージ。
            //   4. 派手なエフェクト。いい感じに。

            // ユニゾン・詠唱を終了
            CtbManager.EndWaitUnison(true, cd, enemyCd);
            CtbManager.EndWaitUnison(false, cd, enemyCd);
            CtbManager.EndMagic(true, cd, enemyCd);
            CtbManager.EndMagic(false, cd, enemyCd);

            // CTB 値を再設定
            // ( waitAction の値を格納 )
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
     * PlayAction() の中身
     * ================================================
     */
     // 通常攻撃・ユニゾンの場合に呼ばれる処理
    private IEnumerator PlayActionNoRamble( Vector2 countActionCharacterInfo )
    {
        // 行動サイドのユニゾン待機を終了
        CtbManager.EndWaitUnison( countActionCharacterInfo.x >= 1, cd, enemyCd);
        // 行動人数の情報を再度取得( コマンド選択前に )
        countActionCharacterInfo =
            CtbManager.CountActionableCharacter(true, cd, enemyCd);

        // コマンド選択を行う( 攻撃・待機・詠唱など )
        //   ┗ selectCommand に、値が入る
        yield return SelectCommand(countActionCharacterInfo);

        // リーダーキャラクターの選択
        yield return SelectLeader(countActionCharacterInfo);

        // 動けるキャラクター全員が行動(攻撃・待機・詠唱)を行う
        yield return CallCharacterAction();
    }

    // コマンド内容をもとに
    // 動けるキャラクターすべての行動(攻撃・ユニゾン・詠唱)を行う
    // 　┗ 呼出元: PlayActionNoRamble
    // 　┗ 条件: selectedCommand selectedLeader が決定済
    //　　　　　　　≒ ( SelectCommand SelectLeader が先に呼ばれる )
    IEnumerator CallCharacterAction()
    {
        // 詠唱中キャラの平均待機値を求めておく
        int avePlayerMagWait = CtbManager.GetAverageMagWait(cd);
        int aveEnemyMagWait = CtbManager.GetAverageMagWait(enemyCd);

        // ターゲット未選択状態( ユニゾンなど 2 体目以降は固定 )
        int targetId = -1;
        for (int i = 0; i < ConstantValue.playerNum; i++)
        {
            // 味方キャラクター。CTB ゲージが 0 の場合。
            if (cd[i].ctbNum <= 0 && !cd[i].isWaitUnison)
            {
                if (selectedCommand == (int)Command.Attack)
                {
                    yield return cd[i].PlayAction(targetId, enemyCd);
                    targetId = cd[i].TargetId;
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
        targetId = -1;
        for (int i = 0; i < ConstantValue.enemyNum; i++)
        {
            // 敵キャラクター。CTB ゲージが 0 の場合。
            if (enemyCd[i].ctbNum <= 0 && !enemyCd[i].isWaitUnison)
            {
                if (selectedCommand == (int)Command.Attack)
                {
                    yield return enemyCd[i].PlayAction(targetId, cd);
                    targetId = enemyCd[i].TargetId;
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
            yield break;
        }

        // 味方キャラが行動できないなら処理終了( 敵キャラの値を設定 )
        if (countActionCharacterInfo.x == 0)
        {
            // 敵キャラクターのリーダーは番号が若いキャラ
            for( int i=0; i<enemyCd.Length; i++)
            {
                if (enemyCd[i].ctbNum == 0) selectedLeader = i;
            }
            yield break;
        }
        // リーダー選択処理の本体を呼ぶ
        yield return DoSelectLeader();
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
                if( !cd[i].isWaitUnison )
                    cd[i].FaceObj.transform.localPosition += movePosTrue;
            }
            for (int i = 0; i <ConstantValue.enemyNum; i++)
            {
                if ( !enemyCd[i].isWaitUnison)
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
}
