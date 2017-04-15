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

    // Use this for initialization
    void Start()
    {
        // AddComponent を使うためのダミーオブジェクト
        GameObject dObj = new GameObject("Cube"); // ダミーオブジェクト

        // オブジェクトを取得
        canvas = GameObject.Find("Canvas"); // Canvas オブジェクトを取得
        cd = new CharacterData[ConstantValue.BATTLE_PLAYER_NUM]; // キャラクター DB 確保
        for (int i = 0; i < ConstantValue.BATTLE_PLAYER_NUM; i++)
        {
            // cd[i] = new CharacterData(); はエラーを吐く
            cd[i] = dObj.AddComponent<CharacterData>(); // オブジェクトを噛ます
        }

        // 一時的に
        Party = new int[ConstantValue.BATTLE_PLAYER_NUM]; // パーティ。(★実際の処理は外部読込)
        Party[0] = 8;
        Party[1] = 7;
        Party[2] = 10;
        Party[3] = 9;

        // 人数分のキャラクターデータの読込
        for (int i = 0; i < ConstantValue.BATTLE_PLAYER_NUM; i++)
        {
            cd[i].LoadCharacterData(Party[i], i);
            cd[i].MakeCharacterGraphic(canvas);
        }

        // メインループスタート
        StartCoroutine("MyUpdate");
    } // --- Start()

    IEnumerator MyUpdate()
    {
        while (true)
        {
            // CTBメータを 1 つ進める
            // 人数分のキャラクターデータの読込
            int characterAction = 0;
            for (int i = 0; i < ConstantValue.BATTLE_PLAYER_NUM; i++)
            {
                // ctbゲージを進め、アクション可能キャラがいたら、フラグを立てる 
                cd[i].ctbNum--;    
                if (cd[i].ctbNum == 0) characterAction++;
            }
            // 移動演出のコルーチンを呼び出す
            yield return CtbMove();

            if( characterAction > 0 ) // アクション可能キャラがいれば
            {
                // ユニゾンはとりあえず知らん
                for (int i = 0; i < ConstantValue.BATTLE_PLAYER_NUM; i++)
                {
                    if (cd[i].ctbNum <= 0)
                        cd[i].ctbNum = UnityEngine.Random.Range(5, 15);
                }
            }

            yield return 0;
        }
    }

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
            for (int i = 0; i < ConstantValue.BATTLE_PLAYER_NUM; i++)
            {
                movePosTrue = movePos * 60 * Time.deltaTime;
                cd[i].FaceObj.transform.localPosition += movePosTrue;
            }
            yield return 0;
        }
        // CTB に応じた位置に再描画
        for (int i = 0; i < ConstantValue.BATTLE_PLAYER_NUM; i++)
        {
            cd[i].SetFaceObj();
        }
        yield return 0;
    }
}
