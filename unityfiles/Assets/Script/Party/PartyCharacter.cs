using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * キャラクターの情報を管理するクラス
 * ( 全てのシーンで利用 )
 */
class PartyCharacter
{
    /* キャラクターを管理するための情報 */
    int characterId; // エクセルから取得する際に利用するID

    /* ステータス情報(フィールド・戦闘の両方で利用するもの) */
    CharacterStatus cs; // エクセルから取得し、レベルにより決まる値

    /* ステータス情報(フィールドのみ) */
    public int level; // キャラクターのレベル
    public int hp; // 体力( 雑魚戦用 )


}

