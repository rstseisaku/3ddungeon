using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMaster : MonoBehaviour
{
    // new を使わないためのダミーオブジェクト(new を使うと Warnigが出る)
    GameObject dObj;

    // 戦闘中利用データ
    private GameObject canvas; // キャンパス(描画先)
    private int[] Party; // キャラクターの ID を表す配列(添字はPtId)
    private CharacterData[] cd; // キャラクターデータの配列
    private EnemyCharacterData[] enemyCd; // 敵キャラクターの配列



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
        ConstantValue.playerNum = 5; // どっかから取ってくる

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
            cd[i].MakeCharacterGraphic(canvas);
        }
    } // --- LoadPlayerChara()
    // 敵の情報を読み込む
    private void LoadEnemyChara()
    {
        ConstantValue.enemyNum = 5; // どっかから取ってくる
        enemyCd = new EnemyCharacterData[ConstantValue.enemyNum]; // キャラクター DB 確保
        for (int i = 0; i <ConstantValue.enemyNum; i++)
        {
            // キャラクターデータの領域確保( new は使わない )
            enemyCd[i] = dObj.AddComponent<EnemyCharacterData>(); // キャラクターデータの領域確保
                                                             
            // キャラクターデータをロード                                              
            enemyCd[i].LoadCharacterData(i, i);

            //  キャラクター画像描画初期化
            enemyCd[i].MakeCharacterGraphic(canvas);
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
            yield return PlayAction(CountActionCharacter());
            yield return AfterAction(); // ( 空っぽでも良いはず )

            yield return 0;
        }
    }

    // 行動可能人数をカウント
    private Vector2 CountActionCharacter()
    {
        // 行動可能人数を算出する
        int PlayerCount = 0;
        int EnemyCount = 0;
        for (int i = 0; i <ConstantValue.playerNum; i++)
        {
            if (cd[i].ctbNum <= 0) PlayerCount++;
        }
        for (int i = 0; i <ConstantValue.enemyNum; i++)
        {
            if (enemyCd[i].ctbNum <= 0) EnemyCount++;
        }
        return new Vector2(PlayerCount, EnemyCount);
    } // --- CountActionCharacter()

    // 実際の行動処理
    IEnumerator PlayAction( Vector2 countActionCharacterInfo )
    {
        // ランブル・ユニゾン・通常アクションの 3 パターンがありえる
        if( countActionCharacterInfo.x * countActionCharacterInfo.y == 0)
        {
            if (countActionCharacterInfo.x + countActionCharacterInfo.y == 1)
            {
                Debug.Log( "通常攻撃" );
                // 通常攻撃
                yield return CallCharacterAction();
            }
            else
            {
                // ユニゾン
                Debug.Log("ユニゾン");

                //  1. PlayAction を連続で再生する。
                yield return CallCharacterAction();

                //  2. 合体攻撃を放つ。

                //  3. 派手なエフェクト。いい感じに。
            }
        }
        else
        {
            Debug.Log("ランブル!");
            // ランブル
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

    // 動けるキャラクターすべての PlayerAction を呼び出す
    IEnumerator CallCharacterAction()
    {
        for (int i = 0; i <ConstantValue.playerNum; i++)
        {
            if (cd[i].ctbNum <= 0)
            {
                yield return cd[i].PlayAction();
            }
        }
        for (int i = 0; i <ConstantValue.enemyNum; i++)
        {
            if (enemyCd[i].ctbNum <= 0)
            {
                yield return enemyCd[i].PlayAction();
            }
        }
    }
  
    // 後処理
    IEnumerator AfterAction()
    {
        // ウェイトを挟む
        for (int i = 0; i < 30; i++)
            yield return 0;

        // CTB に応じた位置に再描画
        for (int i = 0; i <ConstantValue.playerNum; i++)
        {
            cd[i].SetFaceObj();
        }
        for (int i = 0; i <ConstantValue.enemyNum; i++)
        {
            enemyCd[i].SetFaceObj( ConstantValue.BATTLE_ENEMYFACE_OFFSETY , -1);
        }
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
                if (cd[i].ctbNum == 0) isDecideNextChara = true;
            }
            for (int i = 0; i <ConstantValue.enemyNum; i++)
            {
                enemyCd[i].ctbNum--;
                if (enemyCd[i].ctbNum == 0) isDecideNextChara = true;
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
                cd[i].FaceObj.transform.localPosition += movePosTrue;
            }
            for (int i = 0; i <ConstantValue.enemyNum; i++)
            {
                enemyCd[i].FaceObj.transform.localPosition += movePosTrue;
            }
            yield return 0;
        }
        // CTB に応じた位置に再描画
        for (int i = 0; i <ConstantValue.playerNum; i++)
        {
            cd[i].SetFaceObj();
        }
        for (int i = 0; i <ConstantValue.enemyNum; i++)
        {
            enemyCd[i].SetFaceObj( ConstantValue.BATTLE_ENEMYFACE_OFFSETY , -1 );
        }
        yield return 0;
    } // --- CtbMove()
}
