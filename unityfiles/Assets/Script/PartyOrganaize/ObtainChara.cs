using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 取得済のキャラクターを管理
public class ObtainChara : MonoBehaviour
{
    // (ユーザー情報≒ここに割り振ったパラメータも入れてしまう？)


    /* キャラクターを所持済か格納 */
    public bool[] isObtainChara;

    /* ステータス割振による補正値 */
    public static int[] atkAdd; // 攻撃割振値
    public static int[] maxHpAdd; // 魔力値割振

    // 内部で利用する変数
    private static GameObject partyObj;
    private static HaveChara mHaveCharaScript;


    /* セーブデータがなかった場合(入手処理) */
    public void Init()
    {
        // 全てを未取得に
        isObtainChara = new bool[Variables.Unit.Num + 1];
        atkAdd = new int[Variables.Unit.Num + 1];
        maxHpAdd = new int[Variables.Unit.Num + 1];
        for (int i = 0; i < isObtainChara.Length; i++)
        {
            isObtainChara[i] = false;
            atkAdd[i] = i * 50;
            maxHpAdd[i] = i * 50;
        }
        isObtainChara[1] = true;
        isObtainChara[2] = true;
        isObtainChara[3] = true;
    }

    /* 所持しているキャラクターリストを表示 */
    public void DrawHaveParty()
    {
        // インスタンス化
        GameObject canvas = GameObject.Find("PartyCanvas");

        string FilePath = "Prefabs\\Party\\HaveChara";
        partyObj = Utility.MyInstantiate(FilePath, canvas);
        mHaveCharaScript = partyObj.GetComponent<HaveChara>();
        mHaveCharaScript.GenerateObject( isObtainChara );
    }

}