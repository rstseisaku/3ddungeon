﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * キャラクターの情報を管理するクラス
 * ( 全てのシーンで利用 )
 */
public class PartyCharacter : MonoBehaviour
{
    /* キャラクターを管理するための情報 */

    /* ステータス情報(フィールド・戦闘の両方で利用するもの) */
    public int characterId; // エクセルから取得する際に利用するID
    public CharacterStatus cs; // エクセルから取得した値
    public CharacterStatus bcs; // 戦闘時に利用する値(補正値を考慮)

    /* ステータス情報(フィールドのみ) */
    public int hp; // 体力( 雑魚戦用 )

    /* ステータス割振による補正値 */
    public int atkAdd; // 攻撃割振値
    public int maxHpAdd; // 魔力値割振


    /* IDを受け取りキャラの初期化を行う */
    public void Init( int id )
    {
        // キャラクターのIDを受け取る
        characterId = id;
        if (id == -1) return;

        // キャラクターステータス情報のコンポーネント生成
        cs = gameObject.AddComponent<CharacterStatus>();
        bcs = gameObject.AddComponent<CharacterStatus>();

        // .csvから、キャラクターのステータス情報を読み込む(仮)
        string FilePath = Variables.Unit.PlayerDataFilePath;
        cs.LoadCharacterData(FilePath, characterId);
    }

    public CharacterStatus GetBattleCharacerStatus()
    {
        bcs.CopyCharacterStatus(cs);
        bcs.atk += atkAdd;
        bcs.maxHp += maxHpAdd;
        return bcs;
    }
}
