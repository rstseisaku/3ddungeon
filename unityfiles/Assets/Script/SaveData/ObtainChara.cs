using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 取得済のキャラクターを管理
[System.Serializable()]
public class ObtainChara
{
    /* キャラクターを所持済か格納 */
    public bool[] isObtainChara;

    /* ステータス割振による補正値 */
    public int[] atkAdd; // 攻撃割振値
    public int[] maxHpAdd; // 魔力値割振

    /* 編集領域の確保 */
    public void NewVariables()
    {
        isObtainChara = new bool[Variables.Unit.Num + 2];
        atkAdd = new int[Variables.Unit.Num + 2];
        maxHpAdd = new int[Variables.Unit.Num + 2];
    }

    /* セーブデータがなかった場合 */
    public void InitUserData()
    {
        // 全てを未取得に
        for (int i = 0; i < isObtainChara.Length; i++)
        {
            isObtainChara[i] = false;
        }
        isObtainChara[1] = true;
        isObtainChara[2] = true;
        isObtainChara[3] = true;
        isObtainChara[4] = true;
        isObtainChara[5] = true;
    }
}