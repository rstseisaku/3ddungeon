using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerCharacter : BaseCharacter
{
    // キャラクターのデータ読込(味方)
    public void LoadCharacterData(int characterId, int pId)
    {
        // プレイヤーサイド
        isPlayerCharacter = true;
        // パーティーの何番目にいるのかを設定
        partyId = pId;
        // キャラクターデータの読込処理
        string FilePath = "Assets\\Resources\\CharacterData\\data.csv";
        LoadCharacterData(FilePath, characterId);        
    }

    // 通常攻撃。(プレイヤーキャラクター)
    public IEnumerator PlayAction(int target, EnemyCharacterData[] enemyCd, ComboManager cm)
    {
        // 攻撃演出(仮)
        targetId = target;
        yield return DrawBattleGraphic(enemyCd, cm);

        // 攻撃処理(計算)
        yield return Attack(enemyCd, cm);

        // 行動終了処理
        AfterAction();
    }
}
