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
    public IEnumerator PlayAction( int target, EnemyCharacterData[] enemyCd )
    {
        // ユニゾンの場合は、ターゲットが既に決まっている
        targetId = target;
        // -1 の場合のみターゲット決定処理
        if (targetId == -1)
        {
            yield return SelectTarget( enemyCd ); 
        }

        ctbNum = (int)UnityEngine.Random.Range(0, 10);

        // 攻撃処理
        Attack(enemyCd);

        // 攻撃演出(仮)
        yield return DrawBattleGraphic();
    }
}
