using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyCharacterData : CharacterBase
{
    // キャラクターのデータ読込(敵)
    public void LoadCharacterData(int characterId, int pId)
    {
        // パーティーの何番目にいるのかを設定
        partyId = pId;
        // CTB 値を適当に初期化しておく
        ctbNum = (int)UnityEngine.Random.Range(0, 10);
        // キャラクターデータの読込処理
        string FilePath = "Assets\\Resources\\CharacterData\\enemyData.csv";
        LoadCharacterData(FilePath, characterId);
    }

    // 通常攻撃。(敵キャラクター)
    public IEnumerator PlayAction()
    {
        ctbNum = (int)UnityEngine.Random.Range(15, 22);
        yield return DrawBattleGraphic();
    }
}
