using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterData : CharacterBase
{
    // キャラクターのデータ読込(味方)
    public void LoadCharacterData(int characterId, int pId)
    {
        // パーティーの何番目にいるのかを設定
        partyId = pId;
        // キャラクターデータの読込処理
        string FilePath = "Assets\\Resources\\CharacterData\\data.csv";
        LoadCharacterData(FilePath, characterId);
    }

    // 通常攻撃。(プレイヤーキャラクター)
    public IEnumerator PlayAction()
    {
        ctbNum = (int)UnityEngine.Random.Range(0, 10);
        yield return SelectTarget();
        yield return DrawBattleGraphic();
    }
}
