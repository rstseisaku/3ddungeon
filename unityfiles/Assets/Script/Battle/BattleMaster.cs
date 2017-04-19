using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 現在作業: ユニゾン ⇒ 詠唱 でキャラクターがずれるのの修正

enum Command { Attack, Unison, Magic }

public class BattleMaster : MonoBehaviour
{
    // new を使わないためのダミーオブジェクト(new を使うと Warnigが出る)
    GameObject dObj;

    // 戦闘中利用データ
    private GameObject canvas; // キャンパス(描画先)
    private int[] Party; // キャラクターの ID を表す配列(添字はPtId)
    private CharacterData[] cd; // キャラクターデータの配列
    private EnemyCharacterData[] enemyCd; // 敵キャラクターの配列

    // 選択されたコマンド
    private int selectedCommand;


    // Use this for initialization
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



    // ゲームメインループ
    IEnumerator MyUpdate()
    {
        while (true)
        {
            // 行動できるキャラが出てくるまで CTB を進行
            yield return DecideNextActionCharacter();

            // 行動可能キャラクターをカウントし、実際のアクションを行う
            yield return PlayAction();
            yield return AfterAction(); // ( 空っぽでも良いはず )

            yield return 0;
        }
    }

    // 実際の行動処理
    // ( x = 味方行動可能人数 , y = 敵行動可能人数 )
    IEnumerator PlayAction()
    {
        // ランブル判定。( ユニゾン中のキャラは数えない )
        Vector2 countActionCharacterInfo;
        countActionCharacterInfo = CountActionableCharacter(true);
        // Debug.Log(countActionCharacterInfo);

        // ランブル・ユニゾン・通常アクションの 3 パターンがありえる
        if( countActionCharacterInfo.x * countActionCharacterInfo.y == 0)
        {
            // ランブルでないことが確定( ユニゾン・通常アクションのいずれか )

            // 行動サイドのユニゾン待機を終了し、再度カウント
            EndWaitUnison(countActionCharacterInfo.x >= 1);
            countActionCharacterInfo = CountActionableCharacter(true);

            if (countActionCharacterInfo.x + countActionCharacterInfo.y == 1)
            {
                // 敵味方どちらかが単体攻撃を放つ

                // コマンド選択( 攻撃・待機・詠唱など )
                int isMagicEnable = 1;
                int isUnisonEnable = 1;
                if (CountActionableMagic() != 0)
                {
                    isMagicEnable = 0;
                    isUnisonEnable = 0;
                }
                yield return SelectCommand(
                    countActionCharacterInfo.x >= 1,
                    new Vector3(1, isUnisonEnable, isMagicEnable));

                // 実際の行動を呼出
                yield return CallCharacterAction( selectedCommand );
            }
            else
            {
                // 敵味方どちらかがユニゾン攻撃

                // コマンド選択( 攻撃・待機・詠唱など )
                int isMagicEnable = 1;
                int isUnisonEnable = 0;
                if (CountActionableMagic() != 0)
                {
                    isMagicEnable = 0;
                    isUnisonEnable = 0;
                }
                yield return SelectCommand(
                    countActionCharacterInfo.x >= 1,
                    new Vector3(1, isUnisonEnable, isMagicEnable));

                //★  1. PlayAction を連続で再生する。
                // ユニゾン待機終了
                EndWaitUnison(countActionCharacterInfo.x >= 1);

                // 実際の行動を呼出
                yield return CallCharacterAction( selectedCommand );

                //  2. 合体攻撃を放つ。

                //  3. 派手なエフェクト。いい感じに。
            }
        }
        else
        {
            // ランブル

            Debug.Log("ランブル!");
            // 　1. それぞれの魔力レベルを取得
            //   2. 両サイドの和を求める
            //   3. 少ない方にスタン処理＆ダメージ。
            //   4. 派手なエフェクト。いい感じに。

            for (int i = 0; i <ConstantValue.playerNum; i++)
            {
                if (cd[i].ctbNum <= 0) cd[i].ctbNum = 30;
            }
            for (int i = 0; i <ConstantValue.enemyNum; i++)
            {
                if (enemyCd[i].ctbNum <= 0) enemyCd[i].ctbNum = 31;
            }
        }
        yield return 0;
    }

    // 行動可能人数をカウント
    private Vector2 CountActionableCharacter( bool isIgnoreWaitUnison )
    {
        // 行動可能人数を算出する
        int PlayerCount = 0;
        int EnemyCount = 0;
        for (int i = 0; i < ConstantValue.playerNum; i++)
        {
            if ( isIgnoreWaitUnison && cd[i].isWaitUnison) continue;
            if ( cd[i].ctbNum <= 0) PlayerCount++;
        }
        for (int i = 0; i < ConstantValue.enemyNum; i++)
        {
            if ( isIgnoreWaitUnison && enemyCd[i].isWaitUnison) continue;
            if ( enemyCd[i].ctbNum <= 0) EnemyCount++;
        }
        return new Vector2(PlayerCount, EnemyCount);
    } // --- CountActionCharacter()

    // 行動可能キャラで、詠唱しているキャラをカウント
    private int CountActionableMagic()
    {
        // 詠唱中である かつ ユニゾン待機中の敵サイドキャラではない
        // キャラクターをカウントする
        int count = 0;
        for (int i = 0; i < ConstantValue.playerNum; i++)
        {
            if (cd[i].ctbNum <= 0 && cd[i].isMagic && 
                !cd[i].isWaitUnison ) count++;
        }
        for (int i = 0; i < ConstantValue.enemyNum; i++)
        {
            if (enemyCd[i].ctbNum <= 0 && enemyCd[i].isMagic && 
                !enemyCd[i].isWaitUnison ) count++;
        }
        return count;
    }

    // ユニゾン待機を終了。
    // ( ユニゾンの合流に成功したら、CallAction の前に呼び出す )
    private void EndWaitUnison( bool isPlayer )
    {
        for (int i = 0; i < ConstantValue.playerNum && isPlayer; i++)
        {
            if (cd[i].ctbNum <= 0 && cd[i].isWaitUnison)
                cd[i].EndUnison();
        }
        for (int i = 0; i < ConstantValue.enemyNum && !isPlayer; i++)
        {
            if (enemyCd[i].ctbNum <= 0 && enemyCd[i].isWaitUnison)
                enemyCd[i].EndUnison();
        }
    }

    // 行動内容選択(アタック・ユニゾン・攻撃)
    // ┗ vec( int, int, int ) 
    private IEnumerator SelectCommand( bool isPlayer, Vector3 vec )
    {
        if (isPlayer)
        {
            string FilePath = "Prefabs\\Battle\\SelectAction";
            GameObject selObj = (GameObject)Instantiate(Resources.Load(FilePath),
                                new Vector3(-400, 0, 0),
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
        else
        {
            // 通常攻撃しかしない(敵)
            selectedCommand = 0;
        }
    }

    // コマンド内容をもとに
    // 動けるキャラクターすべての行動を行う
    IEnumerator CallCharacterAction( int command )
    {
        int avePlayerMagWait = 0;
        int co = 0;
        for (int j = 0; j < ConstantValue.playerNum; j++)
        {
            if (cd[j].ctbNum <= 0 && !cd[j].isWaitUnison)
            {
                avePlayerMagWait += cd[j].magWait;
                co++;
            }
        }
        if (co != 0)  avePlayerMagWait /= co;

        co = 0;
        int aveEnemyMagWait = 0;
        for (int j = 0; j < ConstantValue.enemyNum; j++)
        {
            if (enemyCd[j].ctbNum <= 0 && !enemyCd[j].isWaitUnison)
            {
                aveEnemyMagWait += enemyCd[j].magWait;
                co++;
            }
        }
        if( co != 0 ) aveEnemyMagWait /= co;


        int targetId = -1;
        for (int i = 0; i <ConstantValue.playerNum; i++)
        {
            // 味方キャラクター。CTB ゲージが 0 の場合。
            if (cd[i].ctbNum <= 0 && !cd[i].isWaitUnison)
            {
                if (command == (int)Command.Attack)
                {
                    yield return cd[i].PlayAction(targetId, enemyCd);
                    targetId = cd[i].TargetId;
                }
                else if (command == (int)Command.Unison)
                {
                    cd[i].StartUnison();
                }
                else if (command == (int)Command.Magic)
                {
                    cd[i].StartMagic( avePlayerMagWait );
                }
            }
        }
        for (int i = 0; i <ConstantValue.enemyNum; i++)
        {
            // 味方キャラクター。CTB ゲージが 0 の場合。
            if (enemyCd[i].ctbNum <= 0 && !enemyCd[i].isWaitUnison)
            {
                if (command == (int)Command.Attack)
                {
                    yield return enemyCd[i].PlayAction(targetId, cd);
                    targetId = enemyCd[i].TargetId;
                }
                else if (command == (int)Command.Unison)
                {
                    enemyCd[i].StartUnison();
                }
                else if (command == (int)Command.Magic)
                {
                    cd[i].StartMagic(aveEnemyMagWait);
                }
            }
        }
    }
  
    // 後処理
    IEnumerator AfterAction()
    {
        // ウェイトを挟む
        yield return Utility.Wait(30);

        // 情報を再描画
        DrawCharacterData();
    }

    // 行動できるキャラクターが出るまでループを回す
    IEnumerator DecideNextActionCharacter()
    {
        bool isDecideNextChara = false; // 行動キャラクター決定フラグ
        while ( !isDecideNextChara )
        {
            // CTBメータを 1 つ進める
            // 人数分のキャラクターデータの読込
            for (int i = 0; i <ConstantValue.playerNum; i++)
            {
                // ctbゲージを進め、アクション可能キャラがいたら、フラグを立てる 
                cd[i].ctbNum--;
                if (cd[i].ctbNum <= 0 && !cd[i].isWaitUnison ) isDecideNextChara = true;
                if (cd[i].ctbNum < 0) cd[i].ctbNum = 0;
            }
            for (int i = 0; i <ConstantValue.enemyNum; i++)
            {
                enemyCd[i].ctbNum--;
                if (enemyCd[i].ctbNum <= 0 && !enemyCd[i].isWaitUnison) isDecideNextChara = true;
                if (enemyCd[i].ctbNum < 0) enemyCd[i].ctbNum = 0;
            }
            // 移動演出のコルーチンを呼び出す
            yield return CtbMove();
        }
    } // --- DecideNextActionCharacter()

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
