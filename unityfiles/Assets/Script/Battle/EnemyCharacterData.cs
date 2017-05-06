using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyCharacterData : BaseCharacter
{
    // キャラクターのデータ読込(敵)
    public void LoadCharacterData(int characterId, int pId)
    {
        // パーティーIDの設定
        partyId = pId;

        // キャラクターデータの読込処理
        string FilePath = Variables.Enemy.EnemyDataFilePath;
        cs = gameObject.AddComponent<CharacterStatus>();
        cs.LoadCharacterData(FilePath, characterId);

        LoadCharacterData(FilePath, characterId);
    }

    // 通常攻撃。(敵キャラクター)
    public IEnumerator PlayAction( int target, PlayerCharacter[] cd, ComboManager cm)
    {
        // ユニゾンの場合は、ターゲットが既に決まっている
        targetId = target;

        // 演出処理
        yield return DrawBattleGraphic(cd,cm);

        // 攻撃処理(計算)
        yield return Attack(cd, cm);
    }
}
